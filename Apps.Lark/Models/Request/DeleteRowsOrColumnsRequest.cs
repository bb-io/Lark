using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Request
{
    public class DeleteRowsOrColumnsRequest
    {
        [Display("Spreadsheet ID")]
        public string SpreadsheetToken { get; set; }

        [Display("Sheet ID")]
        public string SheetId { get; set; }

        [Display("Delete mode")]
        public string InsertMode { get; set; }

        [Display("Start index")]
        public int StartIndex { get; set; }

        [Display("End index")]
        public int EndIndex { get; set; }
    }
}
