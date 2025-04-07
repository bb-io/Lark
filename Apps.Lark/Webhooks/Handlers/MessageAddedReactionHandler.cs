using Apps.Lark.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Webhooks.Handlers
{
    public class MessageAddedReactionHandler : BaseWebhookHandler
    {
        const string SubscriptionEvent = "im.message.reaction.created_v1";
        public MessageAddedReactionHandler(InvocationContext invocationContext) : base(invocationContext, SubscriptionEvent)
        {
        }
    }
}
