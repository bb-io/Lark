using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Lark.Models.Request
{
    public class SendFileRequest
    {

        [Display("Chats ID")]
        [DataSource(typeof(ChatDataSourceHandler))]
        public string? ChatsId { get; set; }

        [Display("User ID")]
        [DataSource(typeof(UsersDataSourceHandler))]
        public string? UserId { get; set; }

        [Display("File")]
        public FileReference FileContent { get; set; }

        [Display("File name")]
        public string FileName { get; set; }
    }
}
