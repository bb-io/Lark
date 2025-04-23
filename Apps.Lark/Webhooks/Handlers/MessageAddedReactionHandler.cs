using Apps.Lark.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

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
