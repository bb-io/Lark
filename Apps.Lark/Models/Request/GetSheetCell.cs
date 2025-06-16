using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class GetSheetCell
    {
        [Display("Cell", Description = "Input the cell for searching, for example:'C1'")]
        public string Cell { get; set; }

    }
}
