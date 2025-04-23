using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Lark.DataSourceHandlers
{
    public class UsersDataSourceHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceHandler
    {
        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest("/contact/v3/users?user_id_type=user_id", Method.Get);
            var response = await larkClient.ExecuteWithErrorHandling<UsersResponse>(request);
            var users = response.Data.Items;

            return users.ToDictionary(user => user.UserId, user => user.Name);
        }
    }
}
