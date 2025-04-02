using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Lark.Actions
{
    [ActionList]
    public class SpreadsheetActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
    {
        private IFileManagementClient FileManagementClient { get; set; } = fileManagementClient;



        [Action("Create spreadsheet", Description = "Create spreadsheet")]
        public async Task<CreateSpreadsheetResponse> CreateSpreadsheet([ActionParameter] CreateSpreadsheetRequest input)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest("/sheets/v3/spreadsheets", Method.Post);

            var body = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(input.SpreadsheetName))
            {
                body["title"] = input.SpreadsheetName;
            }

            if (!string.IsNullOrEmpty(input.FolderToken))
            {
                body["folder_token"] = input.FolderToken;
            }

            request.AddHeader("Content-Type", "application/json");

            request.AddJsonBody(body);

            return  await larkClient.ExecuteWithErrorHandling<CreateSpreadsheetResponse>(request);
        }

        [Action("Find cells", Description = "Find cells in a spreadsheet by a query and optional range")]
        public async Task<FindCellsResponse> FindCells([ActionParameter] FindCellsRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v3/spreadsheets/{spreadsheet.SpreadsheetToken}/sheets/{spreadsheet.SheetId}/find", Method.Post);

            var findCondition = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(input.Range))
            {
                findCondition["range"] = $"{spreadsheet.SheetId}!{input.Range}";
            }

            findCondition["match_case"] = input.MatchCase ?? true;
            findCondition["match_entire_cell"] = input.MatchEntireCell ?? false;
            findCondition["search_by_regex"] = input.SearchByRegex ?? false;
            findCondition["include_formulas"] = input.IncludeFormulas ?? false;

            var body = new Dictionary<string, object>
            {
                { "find_condition", findCondition }
            };

            if (!string.IsNullOrEmpty(input.Query))
            {
                body["find"] = input.Query;
            }
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(body);

            return await larkClient.ExecuteWithErrorHandling<FindCellsResponse>(request);
        }

        [Action("Add rows or columns", Description = "Add rows or columns to a sheet")]
        public async Task<AddRowsOrColumnsResponse> AddRowsOrColumns([ActionParameter] AddRowsOrColumnsRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/dimension_range", Method.Post);

            string majorDimension = input.InsertMode.ToLower().Contains("col") ? "COLUMNS" : "ROWS";

            var body = new
            {
                dimension = new
                {
                    sheetId = spreadsheet.SheetId,
                    majorDimension = majorDimension,
                    length = input.Length
                }
            };

            request.AddJsonBody(body);

            return await larkClient.ExecuteWithErrorHandling<AddRowsOrColumnsResponse>(request);
        }


        [Action("Delete rows or columns", Description = "Delete rows or columns from a sheet")]
        public async Task<DeleteRowsOrColumnsResponse> DeleteRowsOrColumns([ActionParameter] DeleteRowsOrColumnsRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/dimension_range", Method.Delete);

            string majorDimension = input.InsertMode.ToLower().Contains("col") ? "COLUMNS" : "ROWS";

            var body = new Dictionary<string, object>
            {
                { "dimension", new {
                    sheetId = spreadsheet.SheetId,
                    majorDimension = majorDimension,
                    startIndex = input.StartIndex,
                    endIndex = input.EndIndex
                } }
            };

            request.AddJsonBody(body);

            return await larkClient.ExecuteWithErrorHandling<DeleteRowsOrColumnsResponse>(request);
        }


        [Action("Insert rows", Description = "Insert rows with data and shifting the existing ones")]
        public async Task<UpdateRowsResponse> InsertRows([ActionParameter] UpdateRowsRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values_prepend", Method.Post);

            var rows = new List<List<string>>();
            foreach (var row in input.Values)
            {
                var columns = row.Split(',')
                                 .Select(value => value.Trim())
                                 .ToList();
                rows.Add(columns);
            }

            var body = new
            {
                valueRange = new
                {
                    range = $"{spreadsheet.SheetId}!{input.Range}",
                    values = rows
                }
            };
            request.AddJsonBody(body);

            var response = await larkClient.ExecuteWithErrorHandling<UpdateRowsResponse>(request);
            return response;
        }


        [Action("Add or update rows/columns", Description = "Add or update data in rows")]
        public async Task<UpdateRowsResponse> AddOrUpdateRows([ActionParameter] UpdateRowsRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values", Method.Put);

            var rows = new List<List<string>>();
            foreach (var row in input.Values)
            {
                var columns = row.Split(',')
                                 .Select(value => value.Trim())
                                 .ToList();
                rows.Add(columns);
            }

            var body = new
            {
                valueRange = new
                {
                    range = $"{spreadsheet.SheetId}!{input.Range}",
                    values = rows
                }
            };
            request.AddJsonBody(body);

            var response = await larkClient.ExecuteWithErrorHandling<UpdateRowsResponse>(request);
            return response;
        }

        [Action("Get range cells values", Description = "Retrieve values for a specified range of cells in a spreadsheet")]
        public async Task<GetRangeCellsValuesResponse> GetRangeCellsValues([ActionParameter] GetRangeCellsValuesRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values/{spreadsheet.SheetId}!{input.Range}", Method.Get);

            return await larkClient.ExecuteWithErrorHandling<GetRangeCellsValuesResponse>(request);
        }

    }
}
