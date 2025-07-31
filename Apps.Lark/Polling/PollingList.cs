using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Constants;
using Apps.Lark.DataSourceHandlers;
using Apps.Lark.Models.Dtos;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Apps.Lark.Polling.Models;
using Apps.Lark.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Lark.Polling
{
    [PollingEventList]
    public class PollingList(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
    {
        [PollingEvent("On new rows added", "Triggered when new rows are added to the sheet")]
        public async Task<PollingEventResponse<NewRowAddedMemory, NewRowResult>> OnNewRowsAdded(
            PollingEventRequest<NewRowAddedMemory> request,
            [PollingEventParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var values = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values/{spreadsheet.SheetId}", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<GetRangeCellsValuesResponse>(values);

            int currentRowCount = response.Data?.ValueRange?.Values?.Count ?? 0;
            string columnRange = ExtractColumnRange(response.Data?.ValueRange?.Range);

            if (request.Memory == null)
            {
                request.Memory = new NewRowAddedMemory
                {
                    LastRowCount = currentRowCount,
                    LastPollingTime = DateTime.UtcNow,
                    Triggered = false
                };

                return new PollingEventResponse<NewRowAddedMemory, NewRowResult>
                {
                    FlyBird = false,
                    Memory = request.Memory,
                    Result = null
                };
            }

            var memory = request.Memory;
            var newRowsList = new List<NewRow>();

            if (response.Data?.ValueRange?.Values != null && currentRowCount > memory.LastRowCount)
            {
                for (int i = memory.LastRowCount; i < currentRowCount; i++)
                {
                    var rowValues = response.Data.ValueRange.Values[i]
                        .Select(cell => cell?.ToString() ?? string.Empty)
                        .ToList();

                    var newRow = new NewRow
                    {
                        RowIndex = i + 1,
                        RowValues = rowValues,
                        Range = $"{columnRange.Split(':')[0]}{i + 1}:{columnRange.Split(':')[1]}{i + 1}"
                    };
                    newRowsList.Add(newRow);
                }
            }

            memory.LastRowCount = currentRowCount;
            memory.LastPollingTime = DateTime.UtcNow;
            memory.Triggered = newRowsList.Any();

            var newRowResult = new NewRowResult
            {
                NewRows = newRowsList.Any() ? newRowsList : new List<NewRow>(),
                SpreadsheetToken = spreadsheet.SpreadsheetToken,
                SheetId = spreadsheet.SheetId

            };

            return new PollingEventResponse<NewRowAddedMemory, NewRowResult>
            {
                FlyBird = newRowsList.Any(),
                Memory = memory,
                Result = newRowResult
            };
        }


        [PollingEvent("On base table new rows added", "Triggered when new rows are added to base table")]
        public async Task<PollingEventResponse<DateTimeMemory, RecordListResponse>> OnNewRowsAddedToBaseTable(
            PollingEventRequest<DateTimeMemory> request,
            [PollingEventParameter] BaseRequest baseId,
            [PollingEventParameter] BaseTableRequest table)
        {
            // first polling, init memory and return early
            if (request.Memory == null)
            {
                return new PollingEventResponse<DateTimeMemory, RecordListResponse>
                {
                    FlyBird = false,
                    Memory = new DateTimeMemory { LastPollingTime = DateTime.UtcNow },
                    Result = null
                };
            }

            // main logic
            var memory = request.Memory;
            var lastPollingDay = memory.LastPollingTime.Date;
            var pollingStartTime = DateTime.UtcNow;
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(item => item.FieldName, item => item);
            var createdAtFieldSchema = tableSchemaResponse.Data.Items.FirstOrDefault(item => item.FieldTypeId == BaseFieldTypes.DateCreated)
                    ?? throw new PluginMisconfigurationException("Selected table doesn't have a field with 'Date created' type. Add a field with 'Date created' type to the table via Lark user interface.");

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "100");
            searchRecordsRequest.AddJsonBody(GenerateBaseRecordsSearchFilter(lastPollingDay, createdAtFieldSchema.FieldName));
            var recordsResponse = await larkClient.ExecuteWithErrorHandling(searchRecordsRequest);

            var receivedRecords = BaseRecordJsonParser
                .ConvertToRecordsList(recordsResponse.Content ?? "", schemaByFieldName)
                .Where(r =>
                {
                    var createdAtField = r.Fields.FirstOrDefault(f => f.FieldId == createdAtFieldSchema.FieldId);
                    var recordCreatedAt = DateTime.Parse(createdAtField?.FieldValue ?? "").ToUniversalTime();
                    return recordCreatedAt > memory.LastPollingTime;
                })
                .ToList();

            memory.LastPollingTime = pollingStartTime;
            return new PollingEventResponse<DateTimeMemory, RecordListResponse>
            {
                FlyBird = receivedRecords.Count > 0,
                Memory = memory,
                Result = new RecordListResponse()
                {
                    BaseId = baseId.AppId,
                    TableId = table.TableId,
                    Records = receivedRecords,
                }
            };
        }

        [PollingEvent("On base table row updated", "Triggered when an existing base table record or it's field is modified.")]
        public async Task<PollingEventResponse<BaseTableRecordChangedMemory, RecordResponse>> OnBaseTableRecordUpdated(
            PollingEventRequest<BaseTableRecordChangedMemory> request,
            [PollingEventParameter] BaseRequest baseId,
            [PollingEventParameter] BaseTableRequest table,
            [PollingEventParameter, Display("Record ID")] string recordId,
            [PollingEventParameter, Display("Field ID"), DataSource(typeof(BaseTableNonDateFieldIdDataSourceHandler))] string? fieldId,
            [PollingEventParameter, Display("Field expected value")] string? fieldExpectedValue)
        {
            if (string.IsNullOrEmpty(fieldId) != string.IsNullOrEmpty(fieldExpectedValue))
            {
                throw new PluginMisconfigurationException("Both 'Field ID' and 'Field expected value' must be either provided or both must be empty.");
            }

            // first polling, init memory and return early
            if (request.Memory == null)
            {
                return new PollingEventResponse<BaseTableRecordChangedMemory, RecordResponse>
                {
                    FlyBird = false,
                    Result = null,
                    Memory = new BaseTableRecordChangedMemory {
                        LastPollingTime = DateTime.UtcNow,
                        LastObservedFieldValue = string.Empty,
                    },
                };
            }

            // main logic
            var memory = request.Memory;
            var lastPollingDay = memory.LastPollingTime.Date;
            var pollingStartTime = DateTime.UtcNow;
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{recordId}", Method.Get);
            var recordsResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);
            var recordsResponseContent = recordsResponse.Content
                ?? throw new PluginMisconfigurationException("Empty content received for the record.");

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(item => item.FieldName, item => item);

            var receivedRecord = BaseRecordJsonParser.ConvertToRecord(recordsResponseContent, schemaByFieldName)
                ?? throw new PluginMisconfigurationException($"Record with ID '{recordId}' not found in table {table.TableId}.");
            
            var lastModifiedFieldSchema = tableSchemaResponse.Data.Items
                .FirstOrDefault(s => s.FieldTypeId == BaseFieldTypes.LastModifiedDate)
                ?? throw new PluginMisconfigurationException("Selected table doesn't have a field with 'Last modified date' type. Add a field with 'Last modified date' type to the table via Lark user interface.");

            var lastModifiedTimeString = receivedRecord.Fields?
                .FirstOrDefault(f => f.FieldId == lastModifiedFieldSchema.FieldId)?
                .FieldValue;

            var lastModifiedTime = DateTime.Parse(lastModifiedTimeString ?? "").ToUniversalTime();

            var isModifiedSinceLastPolling = lastModifiedTime > memory.LastPollingTime;
            var hasExpectedFieldChanged = true; // we only check this if fieldId and fieldExpectedValue are provided

            if (isModifiedSinceLastPolling && !string.IsNullOrEmpty(fieldId) && !string.IsNullOrEmpty(fieldExpectedValue))
            {
                hasExpectedFieldChanged = false;

                var expectedField = receivedRecord.Fields?.FirstOrDefault(f => f.FieldId == fieldId);
                if (expectedField != null)
                {
                    hasExpectedFieldChanged = expectedField.FieldValue == fieldExpectedValue
                        && memory.LastObservedFieldValue != expectedField.FieldValue;

                    memory.LastObservedFieldValue = expectedField.FieldValue;
                }
            }

            var flyBird = isModifiedSinceLastPolling && hasExpectedFieldChanged;
            memory.LastPollingTime = pollingStartTime;
            return new PollingEventResponse<BaseTableRecordChangedMemory, RecordResponse>
            {
                FlyBird = flyBird,
                Memory = memory,
                Result = flyBird == false
                    ? null
                    : new RecordResponse
                        {
                            BaseId = baseId.AppId,
                            TableId = table.TableId,
                            RecordId = receivedRecord.RecordId,
                            Fields = receivedRecord.Fields ?? [],
                        }
            };
        }

        private static string ExtractColumnRange(string fullRange)
        {
            if (string.IsNullOrEmpty(fullRange))
                return "A:A";

            var rangePart = fullRange.Contains("!") ? fullRange.Split('!')[1] : fullRange;

            var columns = rangePart.Split(':');
            if (columns.Length != 2)
                return "A:A";

            string startColumn = columns[0].TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9', '0');
            string endColumn = columns[1].TrimEnd('1', '2', '3', '4', '5', '6', '7', '8', '9', '0');

            return $"{startColumn}:{endColumn}";
        }

        /// <summary>
        /// Filter format definition:
        /// https://open.larksuite.com/document/uAjLw4CM/ukTMukTMukTM/reference/bitable-v1/app-table-record/search
        /// </summary>
        private static object GenerateBaseRecordsSearchFilter(DateTime lastPollingDay, string fieldName)
        {
            var currentDay = lastPollingDay;
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);

            var conditions = new List<object>();

            // include tomorrow to deal with timezone conversions of time to date
            while (currentDay <= tomorrow)
            {
                var unixTimestamp = ((DateTimeOffset)currentDay).ToUnixTimeMilliseconds().ToString();
                conditions.Add(new
                {
                    field_name = fieldName,
                    @operator = "is",
                    value = new object[]
                    {
                        "ExactDate",
                        unixTimestamp
                    }
                });
                currentDay = currentDay.AddDays(1);
            }

            return new
            {
                automatic_fields = false,
                filter = new
                {
                    conjunction = "or",
                    conditions,
                },
            };
        }
    }
}
