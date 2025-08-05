using Apps.Appname.Api;
using Apps.Lark.Models.Response;
using Apps.Lark.Webhooks.Bridge;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using RestSharp;

namespace Apps.Lark.Webhooks.Handlers.Base
{
    public class BaseWebhookHandler : BaseInvocable, IWebhookEventHandler
    {
        private readonly string _subscriptionEvent;

        public BaseWebhookHandler(InvocationContext invocationContext, string subEvent, string? baseId=null) : base(invocationContext)
        {
            _subscriptionEvent = subEvent;

            if (_subscriptionEvent == "drive.file.bitable_record_changed_v1")
            {
                var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);
                var request = new RestRequest($"drive/v1/files/{baseId}/subscribe?file_type=bitable", Method.Post);

                var response = larkClient.ExecuteWithErrorHandling(request);
            }
        }

        public Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
           
            var bridge = new BridgeService(authenticationCredentialsProviders);
            bridge.Subscribe(_subscriptionEvent, values["payloadUrl"], $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}");

            return Task.CompletedTask;
        }

        public Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
            var bridge = new BridgeService(authenticationCredentialsProviders);
            bridge.Unsubscribe(_subscriptionEvent, values["payloadUrl"], $"{InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/')}");

            return Task.CompletedTask;
        }
    }
}
