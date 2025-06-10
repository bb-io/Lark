using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class GetUserResponse
    {
        [Display("User information")]
        public UserDto UserInfo { get; set; }
    }
}
