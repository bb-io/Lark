using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class FindCellsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public FindCellsData Data { get; set; }
    }

    public class FindCellsData
    {
        [JsonProperty("find_result")]
        public FindResult FindResult { get; set; }
    }

    public class FindResult
    {
        [JsonProperty("matched_cells")]
        public List<string> MatchedCells { get; set; }

        [JsonProperty("matched_formula_cells")]
        public List<string> MatchedFormulaCells { get; set; }

        [JsonProperty("rows_count")]
        public int RowsCount { get; set; }
    }
}
