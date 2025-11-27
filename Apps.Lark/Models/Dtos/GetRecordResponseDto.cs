using Apps.Lark.Models.Response;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Dtos
{
    public class GetRecordResponseDto
    {
        [JsonProperty("code")]
        public int? Code { get; set; }

        [JsonProperty("data")]
        public GetRecordDataDto? Data { get; set; }

        [JsonProperty("msg")]
        public string? Message { get; set; }
    }

    public class GetRecordDataDto
    {
        [JsonProperty("record")]
        public RecordItemDto? Record { get; set; }
    }
}
