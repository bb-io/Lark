using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class GetUserRequest
    {
        [Display("User ID")]
        [DataSource(typeof(UsersDataSourceHandler))]
        public string UserId { get; set; }
    }
}
