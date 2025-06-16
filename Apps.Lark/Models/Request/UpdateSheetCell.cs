using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class UpdateSheetCell
    {
        [Display("Sheet cell", Description = "")]
        public string SheetCell { get; set; }

        [Display("Value")]
        public string Value { get; set; }
    }
}
