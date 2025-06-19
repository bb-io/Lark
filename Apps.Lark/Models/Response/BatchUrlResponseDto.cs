using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class BatchUrlResponseDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public BatchUrlData Data { get; set; }
    }

    public class BatchUrlData
    {
        [JsonProperty("tmp_download_urls")]
        public List<TempDownloadUrlItem> TmpDownloadUrls { get; set; }
    }

    public class TempDownloadUrlItem
    {
        [JsonProperty("file_token")]
        public string FileToken { get; set; }

        [JsonProperty("tmp_download_url")]
        public string TmpDownloadUrl { get; set; }
    }
}
