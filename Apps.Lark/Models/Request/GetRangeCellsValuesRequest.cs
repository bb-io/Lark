using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class GetRangeCellsValuesRequest
    {
        [Display("Range")]
        public string Range { get; set; }
    }
}
