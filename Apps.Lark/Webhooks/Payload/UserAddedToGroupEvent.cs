using Newtonsoft.Json;

namespace Apps.Lark.Webhooks.Payload
{
    public class UserAddedToGroupEvent
    {
        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [JsonProperty("operator_id")]
        public SenderId OperatorId { get; set; }

        [JsonProperty("external")]
        public bool External { get; set; }

        [JsonProperty("operator_tenant_key")]
        public string OperatorTenantKey { get; set; }

        [JsonProperty("users")]
        public List<UserAdded> Users { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("i18n_names")]
        public I18nNames I18nNames { get; set; }
    }

    public class UserAdded
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tenant_key")]
        public string TenantKey { get; set; }

        [JsonProperty("user_id")]
        public SenderId UserId { get; set; }
    }

    public class I18nNames
    {
        [JsonProperty("zh_cn")]
        public string ZhCn { get; set; }

        [JsonProperty("en_us")]
        public string EnUs { get; set; }

        [JsonProperty("ja_jp")]
        public string JaJp { get; set; }
    }
}
