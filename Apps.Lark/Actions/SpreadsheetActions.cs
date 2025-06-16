using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
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
        public async Task<CreateSpreadsheetResult> CreateSpreadsheet([ActionParameter] CreateSpreadsheetRequest input)
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

            var raw = await larkClient.ExecuteWithErrorHandling<CreateSpreadsheetResponse>(request);
            var sp = raw.Data.Spreadsheet;

            return new CreateSpreadsheetResult
            {
                SpreadsheetId = sp.SpreadsheetToken,
                FolderId = sp.FolderToken,
                Title = sp.Title,
                Url = sp.Url
            };
        }

        [Action("Find cells", Description = "Find cells in a spreadsheet by a query and optional range")]
        public async Task<FindCellMatchResponse> FindCells([ActionParameter] FindCellsRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
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

            var response= await larkClient.ExecuteWithErrorHandling<FindCellsResponse>(request);

            return new FindCellMatchResponse { MatchedCells=response.Data.FindResult.MatchedCells, MatchedFormulaCells=response.Data.FindResult.MatchedFormulaCells };
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

        [Action("Update sheet cell", Description = "Updates sheet cell")]
        public async Task<GetCellResponse> UpdateSheetCell([ActionParameter] UpdateSheetCell input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values", Method.Put);

            var body = new
            {
                valueRange = new
                {
                    range = $"{spreadsheet.SheetId}!{input.SheetCell}:{input.SheetCell}",
                    values = new List<List<string>>
                    {
                        new List<string> { input.Value }
                    }
                }
            };
            request.AddJsonBody(body);

            var response = await larkClient.ExecuteWithErrorHandling<UpdateRowsResponse>(request);
            if (!response.Msg.Equals("success"))
            {
                throw new PluginApplicationException("Update cell failed. Please check your inputs and try again");
            }

            var request2 = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values/{spreadsheet.SheetId}!{input.SheetCell}:{input.SheetCell}", Method.Get);
            
            var getValue = await larkClient.ExecuteWithErrorHandling<GetRangeCellsValuesResponse>(request2);

            string cellValue = getValue.Data.ValueRange.Values?
                   .FirstOrDefault()?
                   .FirstOrDefault()?
                   .ToString()
                   ?? string.Empty;

            return new GetCellResponse
            {
                Value = cellValue
            };
        }

        [Action("Get range cells values", Description = "Retrieve values for a specified range of cells in a spreadsheet")]
        public async Task<GetRangeResponse> GetRangeCellsValues([ActionParameter] GetRangeCellsValuesRequest input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values/{spreadsheet.SheetId}!{input.Range}?valueRenderOption=UnformattedValue", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<GetRangeCellsValuesResponse>(request);

            return new GetRangeResponse { MajorDimension=response.Data.ValueRange.MajorDimension,
            Values=response.Data.ValueRange.Values, Range=response.Data.ValueRange.Range,
                Revision = response.Data.ValueRange.Revision,
                SpreadsheetToken = response.Data.SpreadsheetToken,
            };
        }


        [Action("Get sheet cell", Description = "Retrieves value for a specified cell in a spreadsheet")]
        public async Task<GetCellResponse> GetSheetCell([ActionParameter] GetSheetCell input, [ActionParameter] SpreadsheetsRequest spreadsheet)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v2/spreadsheets/{spreadsheet.SpreadsheetToken}/values/{spreadsheet.SheetId}!{input.Cell}:{input.Cell}?valueRenderOption=UnformattedValue", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<GetRangeCellsValuesResponse>(request);

            string cellValue = response.Data.ValueRange.Values?
                   .FirstOrDefault()?           
                   .FirstOrDefault()?            
                   .ToString() 
                   ?? string.Empty;     

            return new GetCellResponse
            {
                Value = cellValue
            };
        }

        [Action("Download Spreadsheet", Description = "Downloads a spreadsheet file by exporting it from Lark Drive.")]
        public async Task<FileResponse> DownloadSpreadsheet([ActionParameter] DownloadSpreadsheetRequest input)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var ticket = await CreateExportTask(larkClient, input.SpreadsheetToken);

            var exportResult = await GetExportTaskResult(larkClient, ticket, input.SpreadsheetToken);
            if (exportResult.JobStatus != 0)
            {
                throw new PluginApplicationException($"Export task failed: {exportResult.JobErrorMsg}");
            }

            var file = await DownloadFile(larkClient, exportResult.FileToken, exportResult.FileName, exportResult.FileExtension);

            return new FileResponse { File = file };
        }
        public async Task<string> CreateExportTask(LarkClient larkClient, string sheetToken)
        {
            var createTaskRequest = new RestRequest("/drive/v1/export_tasks", Method.Post);
            createTaskRequest.AddHeader("Content-Type", "application/json");

            var createTaskBody = new
            {
                file_extension = "xlsx",
                token = sheetToken,
                type = "sheet"
            };
            createTaskRequest.AddJsonBody(createTaskBody);

            var createTaskResponse = await larkClient.ExecuteWithErrorHandling<CreateExportTaskResponse>(createTaskRequest);
            if (createTaskResponse.Data == null || string.IsNullOrWhiteSpace(createTaskResponse.Data.Ticket))
            {
                throw new PluginApplicationException("Failed to create export task.");
            }
            return createTaskResponse.Data.Ticket;
        }

        public async Task<ExportTaskResult> GetExportTaskResult(LarkClient larkClient, string ticket, string sheetToken)
        {
            var getTaskResultRequest = new RestRequest($"/drive/v1/export_tasks/{ticket}?token={sheetToken}", Method.Get);

            var getTaskResultResponse = await larkClient.ExecuteWithErrorHandling<ExportTaskResultResponse>(getTaskResultRequest);
            if (getTaskResultResponse.Data == null || getTaskResultResponse.Data.Result == null)
            {
                throw new PluginApplicationException("Failed to get export task result.");
            }
            return getTaskResultResponse.Data.Result;
        }

        public async Task<FileReference> DownloadFile(LarkClient larkClient, string fileToken, string fileName, string fileExtension)
        {
            var downloadRequest = new RestRequest($"/drive/v1/files/{fileToken}/download", Method.Get);
            var downloadResponse = await larkClient.ExecuteWithErrorHandling<byte[]>(downloadRequest);

            if (downloadResponse == null)
            {
                throw new PluginMisconfigurationException("Failed to download file.");
            }

            FileReference fileReference;

            using (var memoryStream = new MemoryStream(downloadResponse))
            {
                fileReference = await fileManagementClient.UploadAsync(
                    memoryStream,
                    "application/octet-stream",
                    $"{fileName}.{fileExtension}"
                );
            }

            return fileReference;
        }

    }
}
