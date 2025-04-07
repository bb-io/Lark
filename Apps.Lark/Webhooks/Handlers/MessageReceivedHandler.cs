using Apps.Lark.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Webhooks.Handlers
{
    public class MessageReceivedHandler : BaseWebhookHandler
    {
        const string SubscriptionEvent = "im.message.receive_v1";
        public MessageReceivedHandler(InvocationContext invocationContext) : base(invocationContext, SubscriptionEvent) { }
    }
}
