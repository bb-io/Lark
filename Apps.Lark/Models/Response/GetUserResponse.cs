using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class GetUserResponse
    {
        [Display("User information")]
        public UserDto UserInfo { get; set; }

        [Display("User ID for mention")]
        public string Mention
        {
            get
            {
                return $"<at user_id=\"{UserInfo.UserId}\">{UserInfo.Name}</at>";
            }
        }
    }
}
