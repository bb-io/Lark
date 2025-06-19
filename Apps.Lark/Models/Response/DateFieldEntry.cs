using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class DateFieldEntry
    {
        [JsonProperty("field_id")]
        [Display("Field ID")]
        public string FieldId { get; set; }

        [JsonProperty("field_name")]
        [Display("Field name")]
        public string FieldName { get; set; }

        [JsonProperty("date")]
        [Display("Date value")]
        public DateTime Date { get; set; }
    }

    public class DateFieldResponse
    {
        [JsonProperty("date_fields")]
        [Display("Date fields")]
        public List<DateFieldEntry> DateFields { get; set; } = new();
    }
}
