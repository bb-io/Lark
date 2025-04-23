using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class SendMessageResult
    {
        [Display("Message ID")]
        public string MessageId { get; set; }

        [Display("Chat ID")]
        public string ChatId { get; set; }

        [Display("Content")]
        public string Content { get; set; }

        [Display("Create time")]
        public DateTime CreateTime { get; set; }

        [Display("Update time")]
        public DateTime UpdateTime { get; set; }

        [Display("Sender")]
        public Sender Sender { get; set; }
    }

    public class SendMessageResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("msg")]
        [Display("Message")]
        public string Msg { get; set; }
    }
    public class Data
    {
        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("chat_id")]
        [Display("Chat ID")]
        public string ChatId { get; set; }

        [JsonProperty("create_time")]
        private long RawCreateTime { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("message_id")]
        [Display("Message ID")]
        public string MessageId { get; set; }

        [JsonProperty("msg_type")]
        [Display("Message type")]
        public string MsgType { get; set; }

        [JsonProperty("sender")]
        public Sender Sender { get; set; }

        [JsonProperty("update_time")]
        private long RawUpdateTime { get; set; }

        [JsonProperty("updated")]
        public bool Updated { get; set; }

        [Display("Create time")]
        public DateTime CreateTimeUtc => DateTimeOffset
       .FromUnixTimeMilliseconds(RawCreateTime)
       .UtcDateTime;

        [Display("Update time")]
        public DateTime UpdateTimeUtc => DateTimeOffset
            .FromUnixTimeMilliseconds(RawUpdateTime)
            .UtcDateTime;
    }

    public class Body
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class Sender
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("id_type")]
        [Display("ID type")]
        public string IdType { get; set; }

        [JsonProperty("sender_type")]
        [Display("Sender type")]
        public string SenderType { get; set; }

        [JsonProperty("tenant_key")]
        [Display("Tenant key")]
        public string TenantKey { get; set; }
    }
}
