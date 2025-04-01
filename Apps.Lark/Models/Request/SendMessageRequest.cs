using Apps.Appname.Handlers.Static;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Lark.Models.Request
{
    public class SendMessageRequest
    {
        [Display("Receive type ID")]
        [StaticDataSource(typeof(ReceiveIdTypeHandler))]
        public string ReceiveIdType { get; set; }

        [Display("Message")]
        public string MessageText { get; set; }

        [Display("Receive ID")]
        public string ReceiveId { get; set; }
    }
}
