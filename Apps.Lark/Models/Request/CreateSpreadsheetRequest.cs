using Apps.Appname.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Request
{
    public class CreateSpreadsheetRequest
    {
        [Display("Spreadsheet name")]
        public string? SpreadsheetName { get; set; }

        [Display("Folder token")]
        [DataSource(typeof(FolderDataSourceHandler))]
        public string? FolderToken { get; set; }
    }
}
