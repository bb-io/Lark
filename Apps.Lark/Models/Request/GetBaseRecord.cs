using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class GetBaseRecord
    {
        [Display("Row index")]
        public int RowIndex { get; set; }
    }
}
