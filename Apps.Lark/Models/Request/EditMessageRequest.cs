using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class EditMessageRequest
    {
        [Display("Message ID")]
        public string MessageId { get; set; }

        [Display("Message text")]
        public string MessageText { get; set; }
    }
}
