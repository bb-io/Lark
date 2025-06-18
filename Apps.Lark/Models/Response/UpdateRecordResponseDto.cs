using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class UpdateRecordResponseDto
    {
        [JsonProperty("code")]
        public int? Code { get; set; }

        [JsonProperty("data")]
        public UpdateRecordDataDto? Data { get; set; }

        [JsonProperty("msg")]
        public string? Message { get; set; }
    }

    public class UpdateRecordDataDto
    {
        [JsonProperty("record")]
        public RecordItemDto? Record { get; set; }
    }

}
