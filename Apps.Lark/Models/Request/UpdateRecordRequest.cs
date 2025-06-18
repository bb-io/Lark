using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class UpdateRecordRequest
    {
        [Display("Field to update")]
        [DataSource(typeof(BaseTableFieldDataSourceHandler))]
        public string FieldName { get; set; }

        [Display("New value")]
        public string? NewValue { get; set; }

        [Display("New values", Description = "Use this field, only when you update field with multiple options")]
        public IEnumerable<string>? NewValues { get; set; }
    }
}
