using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class MultiOptionResponse
    {
        [Display("Selected values")]
        public IEnumerable<string> SelectedValues { get; set; } = [];
    }
}
