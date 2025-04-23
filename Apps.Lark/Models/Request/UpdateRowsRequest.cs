using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class UpdateRowsRequest
    {
        [Display("Range")]
        public string Range { get; set; }

        [Display("Rows values")]
        public IEnumerable<string> Values { get; set; }
    }
}