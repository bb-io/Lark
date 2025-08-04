using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class BaseAppRecordChanged
    {
        [JsonProperty("file_type")]
        public string FileType { get; set; }

        [JsonProperty("file_token")]
        public string FileToken { get; set; }

        [JsonProperty("table_id")]
        public string TableId { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("operator_id")]
        public UserId OperatorId { get; set; }

        [JsonProperty("action_list")]
        public List<ActionItem> ActionList { get; set; }

        [JsonProperty("subscriber_id_list")]
        public List<UserId> SubscriberIdList { get; set; }

        [JsonProperty("update_time")]
        public long UpdateTime { get; set; }
    }

    public class UserId
    {
        [JsonProperty("union_id")]
        public string UnionId { get; set; }

        [JsonProperty("user_id")]
        public string UserIdValue { get; set; }

        [JsonProperty("open_id")]
        public string OpenId { get; set; }
    }

    public class ActionItem
    {
        [JsonProperty("record_id")]
        public string RecordId { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("before_value")]
        public List<FieldValue> BeforeValue { get; set; }

        [JsonProperty("after_value")]
        public List<FieldValue> AfterValue { get; set; }
    }

    public class FieldValue
    {
        [JsonProperty("field_id")]
        public string FieldId { get; set; }

        [JsonProperty("field_value")]
        public string FieldValueData { get; set; }

        [JsonProperty("field_identity_value")]
        public FieldIdentityValue FieldIdentityValue { get; set; }
    }

    public class FieldIdentityValue
    {
        [JsonProperty("users")]
        public List<UserInfo> Users { get; set; }
    }

    public class UserInfo
    {
        [JsonProperty("user_id")]
        public UserId UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("en_name")]
        public string EnglishName { get; set; }

        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; }
    }
}
