using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Lark.Webhooks.Bridge;

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
