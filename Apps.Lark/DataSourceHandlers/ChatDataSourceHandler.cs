using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Lark.DataSourceHandlers
{
    public class ChatDataSourceHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceHandler
    {

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest("/im/v1/chats?user_id_type=user_id", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<ChatsResponse>(request);

            var chats = response.Data.Items;

            return chats.ToDictionary(chat => chat.ChatId, chat => chat.Name);
        }
    }
}
