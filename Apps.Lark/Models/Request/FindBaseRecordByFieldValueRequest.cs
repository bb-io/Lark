using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class FindBaseRecordByFieldValueRequest
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTableFieldDataSourceHandler))]
        public string FieldId { get; set; }

        [Display("Value")]
        public string Value { get; set; } = string.Empty;
    }
}
