using Blackbird.Applications.Sdk.Common;
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
        public string? FolderToken { get; set; }
    }
}
