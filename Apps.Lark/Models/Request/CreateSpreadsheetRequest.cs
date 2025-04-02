using Apps.Appname.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class CreateSpreadsheetRequest
    {
        [Display("Spreadsheet name")]
        public string? SpreadsheetName { get; set; }

        [Display("Folder ID")]
        [DataSource(typeof(FolderDataSourceHandler))]
        public string? FolderToken { get; set; }
    }
}
