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

        //Get sheet cell



        [Action]
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

    }
}
