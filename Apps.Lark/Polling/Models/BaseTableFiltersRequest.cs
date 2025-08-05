using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Polling.Models
{
    public class BaseTableFiltersRequest
    {
        [DataSource(typeof(BaseTableFieldDataSourceHandler))]
        [Display("Field ID")]
        public string? FieldId { get; set; }

        [Display("Value contains")]
        public string? Value { get; set; }
    }
}
