using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class TextNumberFieldEntry
    {
        [Display("Field ID")]
        public string FieldId { get; set; }

        [Display("Field Name")]
        public string FieldName { get; set; }

        [Display("Text Value")]
        public string? TextValue { get; set; }

        [Display("Number Value")]
        public double? NumberValue { get; set; }
    }

    public class TextNumberFieldResponse
    {
        [Display("Text & Number Entries")]
        public List<TextNumberFieldEntry> Entries { get; set; }
    }
}
