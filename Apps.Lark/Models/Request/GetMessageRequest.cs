using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class GetMessageRequest
    {
        [Display("Message ID")]
        public string MessageId { get; set; }
    }
}
