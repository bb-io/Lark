using Apps.Appname.Api;
using Apps.Appname;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.DataSourceHandlers
{
    public class BaseTableLinkFieldIdDataSourceHandler(InvocationContext invocationContext, [ActionParameter] BaseRequest input,
        [ActionParameter] BaseTableRequest table) : Invocable(invocationContext), IAsyncDataSourceHandler
    {
        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest($"/bitable/v1/apps/{input.AppId}/tables/{table.TableId}/fields", Method.Get);
            var response = await larkClient.ExecuteWithErrorHandling<FieldsResponseDto>(request);
            var fields = response.Data.Items.Where(field => field.Type == 15);

            return fields.ToDictionary(field => field.FieldId, field => $"{field.FieldName} ({field.UiType})");
        }
    }
}