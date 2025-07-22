using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Constants;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Lark.DataSourceHandlers
{
    public class BaseTableTextFieldIdDataSourceHandler(InvocationContext invocationContext, [ActionParameter] BaseRequest input,
        [ActionParameter] BaseTableRequest table) : Invocable(invocationContext), IAsyncDataSourceItemHandler
    {
        public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest($"/bitable/v1/apps/{input.AppId}/tables/{table.TableId}/fields", Method.Get);
            var response = await larkClient.ExecuteWithErrorHandling<FieldsResponseDto>(request);

            var acceptableFieldTypes = new List<int>
            {
                BaseFieldTypes.Multiline,
                BaseFieldTypes.SingleOption,
                BaseFieldTypes.Link
            };

            return response.Data.Items
                .Where(field => acceptableFieldTypes.Contains(field.Type))
                .Select(field => new DataSourceItem(field.FieldId, $"{field.FieldName} ({field.UiType})"));
        }
    }
}