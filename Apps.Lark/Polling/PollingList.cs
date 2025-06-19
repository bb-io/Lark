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
        public async Task<PollingEventResponse<NewRowAddedMemory, RecordsResponse>> OnNewRowsAddedToBaseTable(PollingEventRequest<NewRowAddedMemory> request,
            [PollingEventParameter] BaseRequest baseId,
            [PollingEventParameter] BaseTableRequest table)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var listRecords = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records", Method.Get);

            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(listRecords);

            if (recordsResponse?.Data?.Items == null)
            {
                return new PollingEventResponse<NewRowAddedMemory, RecordsResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory ?? new NewRowAddedMemory(),
                    Result = null
                };
            }

            var validRecords = recordsResponse.Data.Items
                 .Select((item, index) => new { Item = item, Index = index })
                 .Where(record => record.Item.Fields?.Any(kv => kv.Value != null) ?? false)
                 .ToList();

            int currentRowCount = validRecords.Count;

            if (request.Memory == null)
            {
                request.Memory = new NewRowAddedMemory
                {
                    LastRowCount = currentRowCount,
                    LastPollingTime = DateTime.UtcNow,
                    Triggered = false
                };

                return new PollingEventResponse<NewRowAddedMemory, RecordsResponse>
                {
                    FlyBird = false,
                    Memory = request.Memory,
                    Result = null
                };
            }

            var memory = request.Memory;
            var newRecordsList = new List<RecordItemDto>();

            if (currentRowCount > memory.LastRowCount)
            {
                var newRecords = validRecords
                    .Where(record => record.Index >= memory.LastRowCount)
                    .ToList();

                foreach (var record in newRecords)
                {
                    record.Item.RowIndex = record.Index;
                    newRecordsList.Add(record.Item);
                }
            }

            memory.LastRowCount = currentRowCount;
            memory.LastPollingTime = DateTime.UtcNow;
            memory.Triggered = newRecordsList.Any();

            var newRowResult = new RecordsResponse
            {
                Records = newRecordsList.Any() ? newRecordsList : null
            };

            return new PollingEventResponse<NewRowAddedMemory, RecordsResponse>
            {
                FlyBird = newRecordsList.Any(),
                Memory = memory,
                Result = newRowResult
            };
        }




        private string ExtractColumnRange(string fullRange)
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
    }
}
