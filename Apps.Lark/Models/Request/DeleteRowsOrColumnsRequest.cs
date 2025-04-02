using Apps.Lark.DataSourceHandlers.Enum;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Request
{
    public class DeleteRowsOrColumnsRequest
    {
        [Display("Delete mode")]
        [StaticDataSource(typeof(ModeTypeHandler))]
        public string InsertMode { get; set; }

        [Display("Start index")]
        public int StartIndex { get; set; }

        [Display("End index")]
        public int EndIndex { get; set; }
    }
}
