using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Lark.Models.Request
{
    public class UpdateRecordRequest
    {
        [Display("Field to update")]
        [DataSource(typeof(BaseTableFieldDataSourceHandler))]
        public string FieldName { get; set; } = string.Empty;

        [Display("New value", Description = "Use this option, only when you update simple text field")]
        public string? NewValue { get; set; }

        [Display("New values", Description = "Use this option, only when you update field with multiple options")]
        public IEnumerable<string>? NewValues { get; set; }

        [Display("New date value", Description = "Use this option, only when you update date field")]
        public DateTime? NewDateValue { get; set; }

        [Display("New checkbox value", Description = "Use this option, only when you update checkbox field")]
        public bool? NewCheckboxValue { get; set; }

        [Display("Attachment", Description = "Use this option, only when you update the attachment")]
        public FileReference? Attachment { get; set; }

        [Display("New link URL", Description = "Use for Link/URL column")]
        public string? NewLinkUrl { get; set; }
    }
}
