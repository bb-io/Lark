using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Polling.Models
{
    public class NewRowResult
    {
        [Display("New rows")]
        public List<NewRow>? NewRows { get; set; }

        [Display("Spreadsheet ID")]
        public string? SpreadsheetToken { get; set; }

        [Display("Sheet ID")]
        public string? SheetId { get; set; }
    }

    public class NewRow
    {
        [Display("Row index")]
        public int? RowIndex { get; set; }

        [Display("Row values")]
        public List<string>? RowValues { get; set; }

        [Display("Row range")]
        public string? Range { get; set; }
    }
}
