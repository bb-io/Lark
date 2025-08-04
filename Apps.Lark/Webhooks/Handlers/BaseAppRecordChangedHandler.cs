using Apps.Lark.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Lark.Webhooks.Handlers
{
    public class BaseAppRecordChangedHandler : BaseWebhookHandler
    {
        const string SubscriptionEvent = "drive.file.bitable_record_changed_v1";
        public BaseAppRecordChangedHandler(InvocationContext invocationContext) : base(invocationContext, SubscriptionEvent)
        {
        }
    }
}
