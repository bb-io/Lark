using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Dtos;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Apps.Lark.Polling.Models;
using Apps.Lark.Utils;
using Apps.Lark.Webhooks.Handlers;
using Apps.Lark.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using RestSharp;
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

            if (!string.IsNullOrEmpty(filter.FieldId))
            {
                isValid = payload.Event.ActionList.Any(action =>
                {
                    var before = action.BeforeValue
                                   .FirstOrDefault(bv => bv.FieldId == filter.FieldId)
                                   ?.FieldValueData;
                    var after = action.AfterValue
                                   .FirstOrDefault(av => av.FieldId == filter.FieldId)
                                   ?.FieldValueData;

                    if (before == null || after == null || string.Equals(before, after, StringComparison.Ordinal))
                        return false;

                    if (!string.IsNullOrEmpty(filter.Value) && !after.Contains(filter.Value))
                        return false;

                    return true;
                });
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

            var schemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{payload.Event.TableId}/fields", Method.Get);
            var schemaResponse = await Client.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(schemaRequest);
            var schemaByFieldId = schemaResponse.Data.Items.ToDictionary(item => item.FieldId, item => item);

            var beforeRecords = payload.Event.ActionList.Select(action => BaseRecordJsonParser.ConvertToRecord(
                JsonConvert.SerializeObject(new { data = new { record = new { record_id = action.RecordId, fields = action.BeforeValue.ToDictionary(bv => schemaByFieldId[bv.FieldId].FieldName, bv => bv.FieldValueData) } } }),
                schemaByFieldId.ToDictionary(kvp => kvp.Value.FieldName, kvp => kvp.Value)
            )).Where(r => r != null).ToList();

            var afterRecords = payload.Event.ActionList.Select(action => BaseRecordJsonParser.ConvertToRecord(
                JsonConvert.SerializeObject(new { data = new { record = new { record_id = action.RecordId, fields = action.AfterValue.ToDictionary(av => schemaByFieldId[av.FieldId].FieldName, av => av.FieldValueData) } } }),
                schemaByFieldId.ToDictionary(kvp => kvp.Value.FieldName, kvp => kvp.Value)
            )).Where(r => r != null).ToList();

            return new WebhookResponse<UpdatedRecordResponse>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = new UpdatedRecordResponse
                {
                    BaseId = baseId.AppId,
                    TableId = payload.Event.TableId,
                    RecordId = payload.Event.ActionList.First().RecordId,
                    UpdateTime = DateTimeOffset.FromUnixTimeSeconds(payload.Event.UpdateTime).UtcDateTime,
                    BeforeFields = beforeRecords.SelectMany(r => r.Fields).DistinctBy(f => f.FieldId).ToList(),
                    AfterFields = afterRecords.SelectMany(r => r.Fields).DistinctBy(f => f.FieldId).ToList()
                },
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }
    }
}
