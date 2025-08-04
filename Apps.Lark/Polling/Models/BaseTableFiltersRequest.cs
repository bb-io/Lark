using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Polling.Models
{
    public class BaseTableFiltersRequest
    {
        [DataSource(typeof(BaseTableDataSourceHandler))]
        public string? Basetable { get; set; }

        public string? Status { get; set; }

        public string? RecordId { get; set; }
    }
}
