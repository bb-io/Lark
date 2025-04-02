using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class AddRowsOrColumnsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public AddRowsOrColumnsData Data { get; set; }

        [JsonProperty("msg")]
        [Display("Message")]
        public string Msg { get; set; }
    }
    public class AddRowsOrColumnsData
    {
        [JsonProperty("addCount")]
        [Display("Added count")]
        public int AddCount { get; set; }

        [JsonProperty("majorDimension")]
        [Display("Dimension")]
        public string MajorDimension { get; set; }
    }
}
