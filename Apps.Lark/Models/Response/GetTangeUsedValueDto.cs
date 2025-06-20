using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Lark.Models.Response
{
    public class GetRangeUsedValueDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public DataModel Data { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }
    }
    public class DataModel
    {
        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("spreadsheetToken")]
        public string SpreadsheetToken { get; set; }

        [JsonProperty("valueRange")]
        public ValueRangeModel ValueRange { get; set; }
    }

    public class ValueRangeModel
    {
        [JsonProperty("majorDimension")]
        public string MajorDimension { get; set; }

        [JsonProperty("range")]
        public string Range { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("values")]
        public List<List<JToken>> Values { get; set; }
    }
}
