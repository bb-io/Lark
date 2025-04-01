using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class AddRowsOrColumnsRequest
    {
        [Display("Spreadsheet ID")]
        public string SpreadsheetToken { get; set; }

        [Display("Sheet ID")]
        public string SheetId { get; set; }

        [Display("Insert mode")]
        public string InsertMode { get; set; }//row , column

        [Display("Number of rows/cells")]
        public int Length { get; set; }
    }
}
