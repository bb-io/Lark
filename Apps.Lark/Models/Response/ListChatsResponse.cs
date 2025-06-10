using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class ListChatsResponse
    {
        public List<ChatItem> Chats { get; set; } = new List<ChatItem>();

        [Display("User ID for mention")]
        public string Mention
        {
            get
            {
                return string.Join(", ", Chats.Select(chat => $"<at user_id=\"{chat.OwnerId}\"></at>"));
            }
        }
    }
}
