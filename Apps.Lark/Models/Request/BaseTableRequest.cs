using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class BaseTableRequest
    {
        [Display("Table ID")]
        [DataSource(typeof(BaseTableDataSourceHandler))]
        public string TableId { get; set; }
    }
}
