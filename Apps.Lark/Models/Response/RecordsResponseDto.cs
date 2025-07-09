using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Lark.Models.Response
{
    public class RecordsResponseDto
    {
        [JsonProperty("code")]
        public int? Code { get; set; }

        [JsonProperty("data")]
        public RecordsDataDto? Data { get; set; }

        [JsonProperty("msg")]
        public string? Message { get; set; }
    }

    public class RecordsDataDto
    {
        [JsonProperty("has_more")]
        public bool? HasMore { get; set; }

        [JsonProperty("items")]
        public List<RecordItemDto>? Items { get; set; }

        [JsonProperty("total")]
        public int? Total { get; set; }
    }

    public class RecordItemDto
    {
        [JsonProperty("fields")]
        public Dictionary<string, object>? Fields { get; set; }

        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("record_id")]
        [Display("Record ID")]
        public string? RecordId { get; set; }
        


        [Display("Field values")]
        public List<string> FieldValues
        {
            get
            {
                var values = Fields?
                    .Select(kv => kv.Value?.ToString() ?? string.Empty)
                    .ToList()
                    ?? new List<string>();
                return values;
            }
        }
    }

    public class RecordResponse
    {
        public RecordItemDto? Values { get; set; }
    }

    public class RecordsResponse
    {
        public List<RecordItemDto>? Records { get; set; }

        [Display("Records count")]
        public int? RecordsCount { get; set; }
    }
}
