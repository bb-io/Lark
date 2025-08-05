using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Webhooks.Payload
{
    public class UpdatedRecordResponse
    {
        [Display("Base ID")]
        public string BaseId { get; set; } = string.Empty;

        [Display("Table ID")]
        public string TableId { get; set; } = string.Empty;

        [Display("Before records")]
        public IEnumerable<UpdatedRecord> BeforeRecords { get; set; } = [];

        [Display("After records")]
        public IEnumerable<UpdatedRecord> AfterRecords { get; set; } = [];
    }

    public class UpdatedRecord
    {
        [Display("Record ID")]
        public string RecordId { get; set; } = string.Empty;

        [Display("Table ID")]
        public string TableId { get; set; } = string.Empty;

        [Display("File token")]
        public string FileToken { get; set; } = string.Empty;

        [Display("Update time")]
        public DateTime UpdateTime { get; set; }

        [Display("Fields")]
        public List<UpdatedField> Fields { get; set; } = [];
    }

    public class UpdatedField
    {
        [Display("Field ID")]
        public string FieldId { get; set; } = string.Empty;

        [Display("Field value")]
        public string FieldValue { get; set; } = string.Empty;
    }
}
