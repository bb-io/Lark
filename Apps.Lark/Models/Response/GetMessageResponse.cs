using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class GetMessageResult
    {
        [Display("Chat ID")]
        public string ChatId { get; set; }

        [Display("Create time")]
        public DateTime CreateTime { get; set; }

        [Display("Message ID")]
        public string MessageId { get; set; }

        [Display("Sender")]
        public GetMessageSender Sender { get; set; }

        [Display("Body")]
        public GetMessageBody Body { get; set; }
    }

    public class GetMessageResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public GetMessageData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class GetMessageData
    {
        [JsonProperty("items")]
        public List<GetMessageItem> Items { get; set; }
    }

    public class GetMessageItem
    {
        [JsonProperty("body")]
        public GetMessageBody Body { get; set; }

        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [JsonProperty("create_time")]
        private long RawCreateTime { get; set; }

        [JsonProperty("update_time")]
        public long RawUpdateTime { get; set; }

        [Display("Create time")]
        public DateTime CreateTimeUtc => DateTimeOffset
            .FromUnixTimeMilliseconds(RawCreateTime)
            .UtcDateTime;

        [Display("Update time")]
        public DateTime UpdateTimeUtc => DateTimeOffset
            .FromUnixTimeMilliseconds(RawUpdateTime)
            .UtcDateTime;

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("msg_type")]
        public string MsgType { get; set; }

        [JsonProperty("sender")]
        public GetMessageSender Sender { get; set; } 

        [JsonProperty("updated")]
        public bool Updated { get; set; }
    }

    public class GetMessageBody
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class GetMessageSender
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("id_type")]
        public string IdType { get; set; }

        [JsonProperty("sender_type")]
        public string SenderType { get; set; }

        [JsonProperty("tenant_key")]
        public string TenantKey { get; set; }
    }
}
