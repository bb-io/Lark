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
        public async Task<FindCellsResponse> FindCells([ActionParameter] FindCellsRequest input)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v3/spreadsheets/{input.SpreadsheetToken}/sheets/{input.SheetId}/find", Method.Post);

            var findCondition = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(input.Range))
            {
                findCondition["range"] = $"{input.SheetId}!{input.Range}";
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
        public async Task<AddRowsOrColumnsResponse> AddRowsOrColumns([ActionParameter] AddRowsOrColumnsRequest input)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest($"/sheets/v2/spreadsheets/{input.SpreadsheetToken}/dimension_range", Method.Post);

            string majorDimension = input.InsertMode.ToLower().Contains("col") ? "COLUMNS" : "ROWS";

            var body = new
            {
                dimension = new
                {
                    sheetId = input.SheetId,
                    majorDimension = majorDimension,
                    length = input.Length
                }
            };

            request.AddJsonBody(body);

            return await larkClient.ExecuteWithErrorHandling<AddRowsOrColumnsResponse>(request);
        }


        //Delete Rows or Columns
        [Action("Delete rows or columns", Description = "Delete rows or columns from a sheet")]
        public async Task<DeleteRowsOrColumnsResponse> DeleteRowsOrColumns([ActionParameter] DeleteRowsOrColumnsRequest input)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{input.SpreadsheetToken}/dimension_range", Method.Delete);

            string majorDimension = input.InsertMode.ToLower().Contains("col") ? "COLUMNS" : "ROWS";

            var body = new Dictionary<string, object>
            {
                { "dimension", new {
                    sheetId = input.SheetId,
                    majorDimension = majorDimension,
                    startIndex = input.StartIndex,
                    endIndex = input.EndIndex
                } }
            };

            request.AddJsonBody(body);

            return await larkClient.ExecuteWithErrorHandling<DeleteRowsOrColumnsResponse>(request);
        }

    }
}
