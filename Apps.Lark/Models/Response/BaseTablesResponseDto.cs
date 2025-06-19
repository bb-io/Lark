using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class BaseTablesResponseDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public BaseTablesDataDto Data { get; set; }

        [JsonProperty("msg")]
        public string? Message { get; set; }
    }
    public class BaseTablesDataDto
    {
        [JsonProperty("has_more")]
        public bool? HasMore { get; set; }

        [JsonProperty("items")]
        public List<TableItemDto>? Items { get; set; }

        [JsonProperty("page_token")]
        public string PageToken { get; set; }

        [JsonProperty("total")]
        public int? Total { get; set; }
    }

    public class TableItemDto
    {
        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("revision")]
        public int? Revision { get; set; }

        [JsonProperty("table_id")]
        [Display("Table ID")]
        public string? TableId { get; set; }
    }
}
