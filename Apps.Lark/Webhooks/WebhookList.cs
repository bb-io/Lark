using Apps.Appname;
using Apps.Lark.Models.Request;
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
        public async Task<WebhookResponse<UpdatedRecordResponse>> OnBaseTableRecordUpdated(WebhookRequest webhookRequest, [WebhookParameter] BaseRequest baseId,
            [WebhookParameter] BaseTableRequest tableId, [WebhookParameter] BaseTableFiltersRequest filter)
        {
            var payload = JsonConvert.DeserializeObject<BasePayload<BaseAppRecordChanged>>(webhookRequest.Body.ToString());

            if (payload == null)
                throw new Exception("No serializable payload was found in incoming request.");

            bool isValid = true;

            if (!string.IsNullOrEmpty(filter.FieldId) || !string.IsNullOrEmpty(filter.Value))
            {
                isValid = payload.Event.ActionList.Any(action =>
                    action.BeforeValue.Any(bv => bv.FieldId == filter.FieldId && (string.IsNullOrEmpty(filter.Value) || bv.FieldValueData.Contains(filter.Value))) ||
                    action.AfterValue.Any(av => av.FieldId == filter.FieldId && (string.IsNullOrEmpty(filter.Value) || av.FieldValueData.Contains(filter.Value)))
                );
            }

            if (!isValid)
            {
                return new WebhookResponse<UpdatedRecordResponse>
                {
                    HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                    Result = null,
                    ReceivedWebhookRequestType = WebhookRequestType.Default
                };
            }

            var beforeRecords = payload.Event.ActionList.Select(action => new UpdatedRecord
            {
                RecordId = action.RecordId,
                TableId = payload.Event.TableId,
                FileToken = payload.Event.FileToken,
                UpdateTime = DateTimeOffset.FromUnixTimeSeconds(payload.Event.UpdateTime).UtcDateTime,
                Fields = action.BeforeValue.Select(bv => new UpdatedField
                {
                    FieldId = bv.FieldId,
                    FieldValue = bv.FieldValueData
                }).ToList()
            }).ToList();

            var afterRecords = payload.Event.ActionList.Select(action => new UpdatedRecord
            {
                RecordId = action.RecordId,
                TableId = payload.Event.TableId,
                FileToken = payload.Event.FileToken,
                UpdateTime = DateTimeOffset.FromUnixTimeSeconds(payload.Event.UpdateTime).UtcDateTime,
                Fields = action.AfterValue.Select(av => new UpdatedField
                {
                    FieldId = av.FieldId,
                    FieldValue = av.FieldValueData
                }).ToList()
            }).ToList();

            return new WebhookResponse<UpdatedRecordResponse>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = new UpdatedRecordResponse
                {
                    BaseId = baseId.AppId,
                    TableId = payload.Event.TableId,
                    BeforeRecords = beforeRecords,
                    AfterRecords = afterRecords
                },
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }
    }
}
