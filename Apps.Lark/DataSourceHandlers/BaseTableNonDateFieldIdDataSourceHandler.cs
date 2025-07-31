using Apps.Appname.Api;
using Apps.Appname;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common;
using RestSharp;
using Apps.Lark.Constants;

namespace Apps.Lark.DataSourceHandlers
{
    public class BaseTableNonDateFieldIdDataSourceHandler(InvocationContext invocationContext, [ActionParameter] BaseRequest input,
        [ActionParameter] BaseTableRequest table) : Invocable(invocationContext), IAsyncDataSourceHandler
    {
        private readonly HashSet<int> DateFieldTypes = new()
        {
            BaseFieldTypes.Date,
            BaseFieldTypes.DateCreated,
            BaseFieldTypes.LastModifiedDate,
        };

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest($"/bitable/v1/apps/{input.AppId}/tables/{table.TableId}/fields", Method.Get);
            var response = await larkClient.ExecuteWithErrorHandling<FieldsResponseDto>(request);
            var fields = response.Data.Items.Where(field => !DateFieldTypes.Contains(field.Type));

            return fields.ToDictionary(field => field.FieldId, field => $"{field.FieldName} ({field.UiType})");
        }
    }
}