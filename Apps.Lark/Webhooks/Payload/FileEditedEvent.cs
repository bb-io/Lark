using Newtonsoft.Json;

namespace Apps.Lark.Webhooks.Payload
{
    public class FileEditedEvent
    {
        [JsonProperty("file_token")]
        public string FileToken { get; set; }

        [JsonProperty("file_type")]
        public string FileType { get; set; }

        [JsonProperty("operator_id_list")]
        public List<SenderId> OperatorIdList { get; set; }

        [JsonProperty("subscriber_id_list")]
        public List<SenderId> SubscriberIdList { get; set; }
    }
}
