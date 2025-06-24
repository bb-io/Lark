using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class UserInfoByEmailResponse
    {
        [Display("User information")]
        public List<User> UserList { get; set; }

        [Display("Mention user")]
        public string MentionUser
        {
            get
            {
                if (UserList == null || !UserList.Any())
                    return string.Empty;

                return string.Join(" ",
                    UserList.Select(u => $"<at user_id=\"{u.UserId}\"></at>")
                );
            }
        }
    }
    public class UserInfoByEmail
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public DataPayload Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
    public class DataPayload
    {
        [JsonProperty("user_list")]
        public List<User> UserList { get; set; }
    }
    public class User
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("status")]
        public StatusInfo Status { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }
    public class StatusInfo
    {
        [JsonProperty("is_activated")]
        public bool IsActivated { get; set; }

        [JsonProperty("is_exited")]
        public bool IsExited { get; set; }

        [JsonProperty("is_frozen")]
        public bool IsFrozen { get; set; }

        [JsonProperty("is_resigned")]
        public bool IsResigned { get; set; }

        [JsonProperty("is_unjoin")]
        public bool IsUnjoin { get; set; }
    }
}
