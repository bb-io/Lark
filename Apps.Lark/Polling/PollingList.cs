using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Apps.Lark.Polling.Models;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Lark.Polling
{
    [PollingEventList]
    public class PollingList(InvocationContext invocationContext) : Invocable(invocationContext)
    {
        [PollingEvent("On new rows added", "Triggered when new rows are added to the sheet")]
        public async Task<PollingEventResponse<NewRowAddedMemory, NewRowResult>> OnNewRowsAdded(PollingEventRequest<NewRowAddedMemory> request, 
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
        public async Task<PollingEventResponse<DateTimeMemory, RecordIdsResponse>> OnNewRowsAddedToBaseTable(PollingEventRequest<DateTimeMemory> request,
            [PollingEventParameter] BaseRequest baseId,
            [PollingEventParameter] BaseTableRequest table,
            [PollingEventParameter] RecordCreatedRequest input)
        {
            // first polling, init memory and return early
            if (request.Memory == null)
            {
                return new PollingEventResponse<DateTimeMemory, RecordIdsResponse>
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

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "100");
            searchRecordsRequest.AddJsonBody(GenerateBaseRecordsSearchFilter(lastPollingDay, input.FieldName));

            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);
            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsSearchApiResponseDto>(searchRecordsRequest);

            var newRecords = new RecordIdsResponse();
            foreach (var record in recordsResponse.Data.Items)
            {
                if (!record.Fields.ContainsKey(input.FieldName))
                    continue;

                var recordCreatedAt = long.TryParse(record.Fields[input.FieldName], out var ms)
                    ? DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime
                    : DateTime.MinValue;

                if (recordCreatedAt > memory.LastPollingTime)
                    newRecords.RecordIds.Add(new RecordIdDto { RecordId = record.RecordId });
            }

            memory.LastPollingTime = pollingStartTime;
            return new PollingEventResponse<DateTimeMemory, RecordIdsResponse>
            {
                FlyBird = newRecords.RecordIds.Count > 0,
                Memory = memory,
                Result = newRecords
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
                field_names = new[] { fieldName },
                filter = new
                {
                    conjunction = "or",
                    conditions,
                },
            };
        }
    }
}
