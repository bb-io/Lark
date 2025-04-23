using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class DownloadSpreadsheetRequest
    {
        [Display("Spreadsheet ID")]
        [DataSource(typeof(SpreadsheetDataSourceHandler))]
        public string SpreadsheetToken { get; set; }
    }
}
