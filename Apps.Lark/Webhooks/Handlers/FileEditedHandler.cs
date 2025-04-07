using Apps.Lark.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Lark.Webhooks.Handlers
{
    public class FileEditedHandler : BaseWebhookHandler
    {
        const string SubscriptionEvent = "drive.file.edit_v1";
        public FileEditedHandler(InvocationContext invocationContext) : base(invocationContext, SubscriptionEvent)
        {
        }
    }
}
