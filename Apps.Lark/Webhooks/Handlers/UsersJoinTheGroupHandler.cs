using Apps.Lark.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Lark.Webhooks.Handlers
{
    public class UsersJoinTheGroupHandler : BaseWebhookHandler
    {
        const string SubscriptionEvent = "im.chat.member.user.added_v1";
        public UsersJoinTheGroupHandler(InvocationContext invocationContext) : base(invocationContext, SubscriptionEvent)
        {
        }
    }
}
