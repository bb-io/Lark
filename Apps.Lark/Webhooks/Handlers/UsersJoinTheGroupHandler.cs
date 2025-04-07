using Apps.Lark.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Webhooks.Handlers
{
    public class UsersJoinTheGroupHandler :BaseWebhookHandler
    {
        const string SubscriptionEvent = "im.chat.member.user.added_v1";
        public UsersJoinTheGroupHandler(InvocationContext invocationContext) : base(invocationContext, SubscriptionEvent)
        {
        }
    }
}
