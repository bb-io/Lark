using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class FindCellsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public FindCellsData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class FindCellsData
    {
        [JsonProperty("cells")]
        public List<FindCell> Cells { get; set; }
    }

    public class FindCell
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("row")]
        public int Row { get; set; }

        [JsonProperty("column")]
        public int Column { get; set; }
    }
}
