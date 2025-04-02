using Apps.Lark.DataSourceHandlers.Enum;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Lark.Models.Request
{
    public class AddRowsOrColumnsRequest
    {
        [Display("Insert mode")]
        [StaticDataSource(typeof(ModeTypeHandler))]
        public string InsertMode { get; set; }

        [Display("Number of rows/cells")]
        public int Length { get; set; }
    }
}
