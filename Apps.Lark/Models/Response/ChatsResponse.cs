using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class ChatsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public ChatsData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class ChatsData
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("items")]
        public List<ChatItem> Items { get; set; }

        [JsonProperty("page_token")]
        public string PageToken { get; set; }
    }

    public class ChatItem
    {
        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [JsonProperty("chat_status")]
        public string ChatStatus { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("external")]
        public bool External { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("owner_id")]
        public string OwnerId { get; set; }

        [JsonProperty("owner_id_type")]
        public string OwnerIdType { get; set; }

        [JsonProperty("tenant_key")]
        public string TenantKey { get; set; }
    }
}
