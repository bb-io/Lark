using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class GetFieldRequest
    {
        [Display("Field ID")]
        [DataSource(typeof(BaseTableFieldIdDataSourceHandler))]
        public string FieldId { get; set; }
    }
}
