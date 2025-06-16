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
        public async Task<PollingEventResponse<NewRowAddedMemory, IEnumerable<NewRowResult>>> OnNewRowsAdded(PollingEventRequest<NewRowAddedMemory> request, 
            [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var values = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values/{spreadsheet.SheetId}", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<GetRangeCellsValuesResponse>(values);

            int currentRowCount = response.Data?.ValueRange?.Values?.Count ?? 0;


            if (request.Memory == null)
            {
                request.Memory = new NewRowAddedMemory
                {
                    LastRowCount = currentRowCount,
                    LastPollingTime = DateTime.UtcNow,
                    Triggered = false
                };

                return new PollingEventResponse<NewRowAddedMemory, IEnumerable<NewRowResult>>
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
                        Range = $"A{i + 1}:Z{i + 1}" 
                    };
                    newRowsList.Add(newRow);
                }
            }

            memory.LastRowCount = currentRowCount;
            memory.LastPollingTime = DateTime.UtcNow;
            memory.Triggered = newRowsList.Any();


            var result = new List<NewRowResult>();
            if (newRowsList.Any())
            {
                var newRowResult = new NewRowResult
                {
                    NewRows = newRowsList
                };
                result.Add(newRowResult);
            }

            return new PollingEventResponse<NewRowAddedMemory, IEnumerable<NewRowResult>>
            {
                FlyBird = newRowsList.Any(),
                Memory = memory,
                Result = result
            };
        }
    }
}
