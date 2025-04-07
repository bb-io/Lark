using Newtonsoft.Json;

namespace Apps.Lark.Webhooks.Payload
{
    public class ReactionAddedEvent
    {
        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("reaction_type")]
        public ReactionType ReactionType { get; set; }

        [JsonProperty("operator_type")]
        public string OperatorType { get; set; }

        [JsonProperty("user_id")]
        public SenderId UserId { get; set; }

        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("action_time")]
        public long ActionTime { get; set; }
    }
    public class ReactionType
    {
        [JsonProperty("emoji_type")]
        public string EmojiType { get; set; }
    }
}
