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
using Newtonsoft.Json.Linq;
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
            InvocationContext.Logger?.LogInformation("[Lark WebhookLogger] Invoked webhook", null);

            var payloadJson = JsonConvert.SerializeObject(webhookRequest, Formatting.Indented);
            InvocationContext.Logger?.LogInformation(
                $"[Lark WebhookLogger] Payload received from server JSON: {payloadJson}", null);

            var payload = JsonConvert
            .DeserializeObject<BasePayload<BaseAppRecordChanged>>(webhookRequest.Body.ToString())
            ?? throw new Exception("No serializable payload was found in incoming request.");

            var schemaDto = await Client.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(
                new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{payload.Event.TableId}/fields", Method.Get));
            var schemaByFieldId = schemaDto.Data.Items.ToDictionary(i => i.FieldId, i => i);
            var schemaByFieldName = schemaDto.Data.Items.ToDictionary(i => i.FieldName, i => i);

            if (!string.IsNullOrEmpty(filter.FieldId))
            {
                var passes = payload.Event.ActionList.Any(action =>
                {
                    if (!schemaByFieldName.TryGetValue(filter.FieldId, out var schemaItem))
                        return false;

                    var beforeRaw = action.BeforeValue
                                         .FirstOrDefault(bv => bv.FieldId == schemaItem.FieldId)
                                         ?.FieldValueData;
                    var afterRaw = action.AfterValue
                                         .FirstOrDefault(av => av.FieldId == schemaItem.FieldId)
                                         ?.FieldValueData;

                    if (beforeRaw == null || afterRaw == null
                        || string.Equals(beforeRaw, afterRaw, StringComparison.Ordinal))
                        return false;

                    JToken afterToken;
                    try { afterToken = JToken.Parse(afterRaw); }
                    catch { afterToken = JValue.CreateString(afterRaw); }

                    var afterText = BaseRecordJsonParser
                        .ConvertFieldToString(afterToken, schemaItem.FieldTypeId);

                    return string.IsNullOrEmpty(filter.Value)
                        || afterText.Contains(filter.Value, StringComparison.OrdinalIgnoreCase);
                });

                InvocationContext.Logger?.LogInformation($"[Lark WebhookLogger] Filter passes: {passes}", null);

                if (!passes)
                    return PreflightResponse<UpdatedRecordResponse>();
            }

            InvocationContext.Logger?.LogInformation("[Lark WebhookLogger] Checkpoint: building before/after records", null);

            var beforeRecords = payload.Event.ActionList
                .Select(action => BaseRecordJsonParser.ConvertToRecord(
                    JsonConvert.SerializeObject(new
                    {
                        data = new
                        {
                            record = new
                            {
                                record_id = action.RecordId,
                                fields = action.BeforeValue
                                    .ToDictionary(
                                        bv => schemaByFieldId[bv.FieldId].FieldName,
                                        bv => bv.FieldValueData)
                            }
                        }
                    }),
                    schemaByFieldName))
                .Where(r => r != null)
                .ToList();

            var afterRecords = payload.Event.ActionList
                .Select(action => BaseRecordJsonParser.ConvertToRecord(
                    JsonConvert.SerializeObject(new
                    {
                        data = new
                        {
                            record = new
                            {
                                record_id = action.RecordId,
                                fields = action.AfterValue
                                    .ToDictionary(
                                        av => schemaByFieldId[av.FieldId].FieldName,
                                        av => av.FieldValueData)
                            }
                        }
                    }),
                    schemaByFieldName))
                .Where(r => r != null)
                .ToList();

            var result = new UpdatedRecordResponse
            {
                BaseId = baseId.AppId,
                TableId = payload.Event.TableId,
                RecordId = payload.Event.ActionList.First().RecordId,
                UpdateTime = DateTimeOffset.FromUnixTimeSeconds(payload.Event.UpdateTime).UtcDateTime,
                BeforeFields = beforeRecords.SelectMany(r => r.Fields).DistinctBy(f => f.FieldId).ToList(),
                AfterFields = afterRecords.SelectMany(r => r.Fields).DistinctBy(f => f.FieldId).ToList()
            };

            InvocationContext.Logger?.LogInformation(
                $"[Lark WebhookLogger] Completed (RecordId={result.RecordId}; BeforeFields={result.BeforeFields.Count}; AfterFields={result.AfterFields.Count})", null);

            return new WebhookResponse<UpdatedRecordResponse>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = result,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }

        private static WebhookResponse<T> PreflightResponse<T>()
        where T : class
        {
            return new WebhookResponse<T>
            {
                ReceivedWebhookRequestType = WebhookRequestType.Preflight,
                Result = null
            };
        }
    }
}
