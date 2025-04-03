using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Lark.DataSourceHandlers
{
    public class SheetDataSourceHandler : Invocable, IAsyncDataSourceHandler
    {
        private readonly string _spreadsheetToken;

        public SheetDataSourceHandler(InvocationContext invocationContext,
        [ActionParameter] SpreadsheetsRequest input) : base(invocationContext)
        {
            _spreadsheetToken = input.SpreadsheetToken;
        }
        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"/sheets/v3/spreadsheets/{_spreadsheetToken}/sheets/query", Method.Get);
            var response = await larkClient.ExecuteWithErrorHandling<SheetsResponse>(request);
            var sheets = response.Data.Sheets;

            return sheets.ToDictionary(sheet => sheet.SheetId , sheet => sheet.Title);
        }
    }
}
