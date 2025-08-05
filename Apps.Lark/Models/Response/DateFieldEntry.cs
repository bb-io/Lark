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
        [Display("Date value")]
       public DateTime? DateValue { get; set; }
       
    }
}
