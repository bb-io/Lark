using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Lark.DataSourceHandlers
{
    public class BaseTableDataSourceHandler(InvocationContext invocationContext, [ActionParameter] [PollingEventParameter] BaseRequest input) : Invocable(invocationContext), IAsyncDataSourceItemHandler
    {
        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest($"/bitable/v1/apps/{input.AppId}/tables", Method.Get);
            var response = await larkClient.ExecuteWithErrorHandling<BaseTablesResponseDto>(request);
            var tables = response.Data.Items ?? [];

            return tables.Select(table => new DataSourceItem(table.TableId ?? string.Empty, table.Name ?? string.Empty));
        }
    }
}
