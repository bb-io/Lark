using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class DeleteRowsOrColumnsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public DeleteRowsOrColumnsData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
    public class DeleteRowsOrColumnsData
    {
        [JsonProperty("delCount")]
        [Display("Deleted count")]
        public int DelCount { get; set; }

        [JsonProperty("majorDimension")]
        [Display("Dimension")]
        public string MajorDimension { get; set; }
    }
}
