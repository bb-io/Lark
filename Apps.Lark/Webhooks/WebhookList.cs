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
using System.Text.Json;
using System.Text.RegularExpressions;

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

            var payloadJson = JsonConvert.SerializeObject(new
            {
                Headers = webhookRequest.Headers,
                HttpMethod = webhookRequest.HttpMethod?.Method,
                Body = PrepareBodyForLog(webhookRequest.Body),
                Url = webhookRequest.Url,
                QueryParameters = webhookRequest.QueryParameters
            }, Formatting.Indented);

            InvocationContext.Logger?.LogInformation(
                $"[Lark WebhookLogger] Payload received from server JSON: {payloadJson}", null);

            var payload = JsonConvert
                .DeserializeObject<BasePayload<BaseAppRecordChanged>>(GetRawBody(webhookRequest.Body))
                ?? throw new Exception("No serializable payload was found in incoming request.");

            var schemaDto = await Client.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(
                new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{payload.Event.TableId}/fields", Method.Get));

            var schemaByFieldId = schemaDto.Data.Items.ToDictionary(i => i.FieldId, i => i);
            var schemaByFieldName = schemaDto.Data.Items
                .ToDictionary(i => i.FieldName, i => i, StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(filter.FieldId))
            {
                var fieldKey = filter.FieldId.Trim();

                schemaByFieldId.TryGetValue(fieldKey, out var schemaItem);
                if (schemaItem == null)
                {
                    var cleanedName = Regex.Replace(fieldKey, @"\s*\([^)]+\)\s*$", "").Trim();
                    schemaByFieldName.TryGetValue(cleanedName, out schemaItem);
                }

                if (schemaItem == null)
                {
                    InvocationContext.Logger?.LogInformation(
                        $"[Lark WebhookLogger] Filter field not found. Provided='{filter.FieldId}'", null);
                    return PreflightResponse<UpdatedRecordResponse>();
                }

                var passes = false;

                foreach (var action in payload.Event.ActionList)
                {
                    var afterRaw = action.AfterValue
                        .FirstOrDefault(av => av.FieldId == schemaItem.FieldId)?.FieldValueData;

                    string valueSource = "after";

                    if (afterRaw == null)
                    {
                        afterRaw = await GetRecordFieldRawAsync(baseId.AppId, payload.Event.TableId, action.RecordId, schemaItem.FieldId);
                        valueSource = "live";
                    }

                    if (afterRaw == null)
                    {
                        InvocationContext.Logger?.LogInformation(
                            $"[Lark WebhookLogger] Filter: field '{schemaItem.FieldName}' has no value (action={action.RecordId})",
                            null);
                        continue;
                    }

                    var afterText = ToDisplayStringForFilter(afterRaw, schemaItem);

                    InvocationContext.Logger?.LogInformation(
                        $"[Lark WebhookLogger] Filter debug: field='{schemaItem.FieldName}' type='{schemaItem.FieldTypeId}' source='{valueSource}' raw='{afterRaw}' text='{afterText}' compareTo='{filter.Value}'",
                        null);

                    if (string.IsNullOrWhiteSpace(filter.Value))
                    {
                        passes = true;
                        break;
                    }

                    if (afterText.Contains(filter.Value, StringComparison.OrdinalIgnoreCase)
                        || afterRaw.Contains(filter.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        passes = true;
                        break;
                    }
                }

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
                $"[Lark WebhookLogger] Completed (RecordId={result.RecordId}; BeforeFields={result.BeforeFields.Count}; AfterFields={result.AfterFields.Count})",
                null);

            return new WebhookResponse<UpdatedRecordResponse>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = result,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };
        }

        private async Task<string?> GetRecordFieldRawAsync(string appId, string tableId, string recordId, string fieldId)
        {
            try
            {
                var req = new RestRequest($"bitable/v1/apps/{appId}/tables/{tableId}/records/{recordId}", Method.Get);
                var obj = await Client.ExecuteWithErrorHandling<JObject>(req);

                var fields = obj["data"]?["record"]?["fields"] as JObject;
                var token = fields?[fieldId];

                if (token == null) return null;
                return token.Type == JTokenType.String
                    ? token.Value<string>()
                    : token.ToString(Formatting.None);
            }
            catch (Exception ex)
            {
                InvocationContext.Logger?.LogInformation(
                    $"[Lark WebhookLogger] GetRecordFieldRawAsync failed: {ex.Message}", null);
                return null;
            }
        }

        private static string ToDisplayStringForFilter(string raw, dynamic schemaItem)
        {
            if (LooksLikeOptId(raw) && SchemaHasOptions(schemaItem))
            {
                var optId = raw.Trim('"');
                var text = TryResolveOptionText(schemaItem, optId);
                return text ?? optId;
            }

            if (raw.TrimStart().StartsWith("[") && SchemaHasOptions(schemaItem))
            {
                try
                {
                    var ids = JsonConvert.DeserializeObject<List<string>>(raw) ?? new();
                    var names = ids.Select(id => TryResolveOptionText(schemaItem, id) ?? id);
                    return string.Join(", ", names);
                }
                catch {}
            }

            try
            {
                JToken token;
                try { token = JToken.Parse(raw); }
                catch { token = JValue.CreateString(raw); }
                return BaseRecordJsonParser.ConvertFieldToString(token, schemaItem.FieldTypeId);
            }
            catch { return raw; }
        }

        private static bool LooksLikeOptId(string s)
            => s != null && s.Trim('"').StartsWith("opt", StringComparison.OrdinalIgnoreCase);

        private static bool SchemaHasOptions(dynamic schemaItem)
        {
            try
            {
                var prop = AsJToken(schemaItem?.Property);
                if (prop == null) return false;

                return prop.SelectTokens("$..options[*]").Any()
                    || prop.SelectTokens("$..option_list[*]").Any()
                    || prop.SelectTokens("$..enum_options[*]").Any()
                    || prop.SelectTokens("$..choices[*]").Any();
            }
            catch { return false; }
        }

        private static string? TryResolveOptionText(dynamic schemaItem, string optId)
        {
            try
            {
                var prop = AsJToken(schemaItem?.Property);
                if (prop == null) return null;

                foreach (var path in new[] { "$..options[*]", "$..option_list[*]", "$..enum_options[*]", "$..choices[*]" })
                {
                    foreach (var o in prop.SelectTokens(path))
                    {
                        var id = o?["id"]?.Value<string>() ?? o?["value"]?.Value<string>();
                        if (!string.Equals(id, optId, StringComparison.Ordinal)) continue;

                        var textToken = o?["text"] ?? o?["name"] ?? o?["title"];
                        if (textToken == null) return null;

                        if (textToken.Type == JTokenType.String)
                            return textToken.Value<string>();

                        if (textToken.Type == JTokenType.Object)
                        {
                            var obj = (JObject)textToken;
                            var preferredKeys = new[] { "en_us", "en", "zh_cn", "zh", "ja_jp" };
                            foreach (var k in preferredKeys)
                                if (obj.TryGetValue(k, StringComparison.OrdinalIgnoreCase, out var v) && !string.IsNullOrWhiteSpace(v?.ToString()))
                                    return v!.ToString();

                            var first = obj.Properties().Select(p => p.Value?.ToString()).FirstOrDefault(v => !string.IsNullOrWhiteSpace(v));
                            if (!string.IsNullOrWhiteSpace(first)) return first;
                        }
                    }
                }
            }
            catch {}

            return null;
        }

        private static JToken? AsJToken(object? value)
        {
            if (value == null) return null;
            if (value is JToken jt) return jt;
            if (value is string s)
            {
                try { return JToken.Parse(s); } catch { return null; }
            }
            try { return JToken.FromObject(value); } catch { return null; }
        }

        private static string GetRawBody(object? body)
        {
            if (body == null) return string.Empty;
            if (body is string s) return s;
            if (body is JObject jo) return jo.ToString(Formatting.None);
            if (body is JToken jt) return jt.ToString(Formatting.None);
            if (body is JsonElement je) return je.GetRawText();
            return body.ToString() ?? string.Empty;
        }

        private static object? PrepareBodyForLog(object? body)
        {
            var raw = GetRawBody(body);
            if (string.IsNullOrWhiteSpace(raw)) return null;
            try { return JToken.Parse(raw); }
            catch { return raw; }
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
