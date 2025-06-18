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
        public string? RecordId { get; set; }
        
        public int RowIndex { get; set; }

        public List<string> FieldValues
        {
            get
            {
                var values = Fields?
                    .Select(kv => kv.Value?.ToString() ?? string.Empty)
                    .ToList()
                    ?? new List<string>();

                values.Insert(0, RowIndex.ToString());
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
    }
}
