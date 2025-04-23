using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class ErrorResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }
    }
}
