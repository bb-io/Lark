using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class SearchChatsOptions
    {
        [Display("Owner ID")]
        [DataSource(typeof(UsersDataSourceHandler))]
        public string? UserID { get; set; }

        [Display("Chat ID")]
        [DataSource(typeof(ChatDataSourceHandler))]
        public string? ChatID { get; set; }
    }
}
