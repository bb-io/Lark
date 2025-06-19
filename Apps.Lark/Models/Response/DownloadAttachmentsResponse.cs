using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class DownloadAttachmentsResponse
    {
        [JsonProperty("files")]
        public List<FileResponse> Files { get; set; } = new();
    }
}
