using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class SpreadsheetsRequest
    {
        [Display("Spreadsheet ID")]
        public string SpreadsheetToken { get; set; }

        [Display("Sheet ID")]
        public string SheetId { get; set; }
    }
}
