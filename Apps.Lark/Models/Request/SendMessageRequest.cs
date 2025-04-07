using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class SendMessageRequest
    {
        [Display("Message")]
        public string MessageText { get; set; }

        [Display("Chats ID")]
        [DataSource(typeof(ChatDataSourceHandler))]
        public string? ChatsId { get; set; }

        [Display("User ID")]
        [DataSource(typeof(UsersDataSourceHandler))]
        public string? UserId { get; set; }

    }
}
