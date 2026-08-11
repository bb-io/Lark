using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class GetUserResponse
    {
        [Display("User information")]
        public UserDto? UserInfo { get; set; }

        [Display("Mention user")]
        public string MentionUser
        {
            get
            {
                var userInfo = UserInfo;
                if (string.IsNullOrEmpty(userInfo?.UserId))
                    return string.Empty;

                return $"<at user_id=\"{userInfo.UserId}\">{userInfo.Name}</at>";
            }
        }
    }
}
