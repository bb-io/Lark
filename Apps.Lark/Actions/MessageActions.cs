using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Appname.Actions;

[ActionList]
public class MessageActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Send message", Description = "Send message")]
    public async Task<SendMessageResponse> SendMessage([ActionParameter] SendMessageRequest input)
    {
        var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

        var request = new RestRequest("/im/v1/messages", Method.Post);

        request.AddQueryParameter("receive_id_type", input.ReceiveIdType);

        var contentJson = JsonConvert.SerializeObject(new { text = input.MessageText });

        request.AddJsonBody(new
        {
            receive_id = input.ReceiveId,
            msg_type = "text",
            content = contentJson
        });

        var response = await larkClient.ExecuteWithErrorHandling<SendMessageResponse>(request);
        return response;
    }

    //[Action("Send file", Description = "Send message")]
    //public async Task<> SendFile([ActionParameter] SendMessageRequest input)
    //{
       
    //}
}