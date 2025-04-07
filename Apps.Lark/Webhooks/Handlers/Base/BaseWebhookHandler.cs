using Apps.Lark.Webhooks.Bridge;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Lark.Webhooks.Handlers.Base
{
    public class BaseWebhookHandler : BaseInvocable, IWebhookEventHandler
    {
        private readonly string _subscriptionEvent;

        public BaseWebhookHandler(InvocationContext invocationContext, string subEvent) : base(invocationContext)
        {
            _subscriptionEvent = subEvent;
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
