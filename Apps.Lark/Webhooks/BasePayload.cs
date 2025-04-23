using Newtonsoft.Json;

namespace Apps.Lark.Webhooks
{
    public class BasePayload<T>
    {
        [JsonProperty("schema")]
        public string Schema { get; set; }

        [JsonProperty("header")]
        public WebhookHeader Header { get; set; }

        [JsonProperty("event")]
        public T Event { get; set; }
    }
    public class WebhookHeader
    {
        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("create_time")]
        public long CreateTime { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("tenant_key")]
        public string TenantKey { get; set; }

        [JsonProperty("app_id")]
        public string AppId { get; set; }
    }

}
