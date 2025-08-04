using Apps.Appname;
using Apps.Lark.Models.Response;
using Apps.Lark.Polling.Models;
using Apps.Lark.Webhooks.Handlers;
using Apps.Lark.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using System.Net;

namespace Apps.Lark.Webhooks
{
    [WebhookList]
    public class WebhookList : Invocable
    {
        public WebhookList(InvocationContext invocationContext) : base(invocationContext)
        {
        }


        [Webhook("On message received", typeof(MessageReceivedHandler), Description = "This event is triggered when the bot receives a message sent by a user.")]
        public async Task<WebhookResponse<MessageReceiveEvent>> OnChannelReceiveMessage(WebhookRequest webhookRequest)
        {
            var payload = JsonConvert.DeserializeObject<BasePayload<MessageReceiveEvent>>(webhookRequest.Body.ToString());

            if (payload == null)
                throw new Exception("No serializable payload was found in incoming request.");

            return new WebhookResponse<MessageReceiveEvent>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = payload.Event,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };

        }

        [Webhook("On reaction added", typeof(MessageAddedReactionHandler), Description = "This event is triggered when a reaction for a message is added.")]
        public async Task<WebhookResponse<ReactionAddedEvent>> OnReactionMessageAdded(WebhookRequest webhookRequest)
        {
            var payload = JsonConvert.DeserializeObject<BasePayload<ReactionAddedEvent>>(webhookRequest.Body.ToString());

            if (payload == null)
                throw new Exception("No serializable payload was found in incoming request.");

            return new WebhookResponse<ReactionAddedEvent>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = payload.Event,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }

        [Webhook("On user added to group", typeof(UsersJoinTheGroupHandler), Description = "This event is triggered when a new user joins a group (including topic group).")]
        public async Task<WebhookResponse<UserAddedToGroupEvent>> OnUserAdded(WebhookRequest webhookRequest)
        {
            var payload = JsonConvert.DeserializeObject<BasePayload<UserAddedToGroupEvent>>(webhookRequest.Body.ToString());

            if (payload == null)
                throw new Exception("No serializable payload was found in incoming request.");

            return new WebhookResponse<UserAddedToGroupEvent>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = payload.Event,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }


        [Webhook("On file edited", typeof(FileEditedHandler), Description = "This event is triggered when a file is edited.")]
        public async Task<WebhookResponse<FileEditedEvent>> OnFileEdited(WebhookRequest webhookRequest)
        {
            var payload = JsonConvert.DeserializeObject<BasePayload<FileEditedEvent>>(webhookRequest.Body.ToString());

            if (payload == null)
                throw new Exception("No serializable payload was found in incoming request.");

            return new WebhookResponse<FileEditedEvent>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = payload.Event,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }



        [Webhook("On base table record updated", typeof(BaseAppRecordChangedHandler), Description = "This event is triggered when the base table record updates")]
        public async Task<WebhookResponse<BaseAppRecordChanged>> OnBaseTableRecordUpdated(WebhookRequest webhookRequest, [WebhookParameter] BaseTableFiltersRequest filter)
        {
            var payload = JsonConvert.DeserializeObject<BasePayload<BaseAppRecordChanged>>(webhookRequest.Body.ToString());

            if (payload == null)
                throw new Exception("No serializable payload was found in incoming request.");

            bool isValid = true;

            if (!string.IsNullOrEmpty(filter.RecordId))
            {
                isValid = payload.Event.ActionList.Any(action => action.RecordId == filter.RecordId);
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                isValid = isValid && payload.Event.ActionList.Any(action => action.Action == filter.Status);
            }

            if (!isValid)
            {
                return new WebhookResponse<BaseAppRecordChanged>
                {
                    HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                    Result = null,
                    ReceivedWebhookRequestType = WebhookRequestType.Default
                };
            }

            return new WebhookResponse<BaseAppRecordChanged>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = payload.Event,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };

        }
    }
}
