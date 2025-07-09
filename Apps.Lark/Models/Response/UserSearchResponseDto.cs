using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class UserSearchResponseDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public UserSearchDataDto Data { get; set; }
    }

    public class UserSearchDataDto
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("page_token")]
        public string PageToken { get; set; }

        [JsonProperty("users")]
        public List<UserDataDto> Users { get; set; }
    }

    public class UserDataDto
    {
        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("open_id")]
        public string OpenId { get; set; }

        [JsonProperty("department_ids")]
        public List<string> DepartmentIds { get; set; }

        [JsonProperty("avatar")]
        public AvatarDto Avatar { get; set; }
    }
}
