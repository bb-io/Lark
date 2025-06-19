using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Response
{
    public class AttachmentResponse
    {
        [JsonProperty("file_token")]
        public string FileToken { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }
    }

    public class MediaUploadResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public MediaUploadData Data { get; set; }
    }

    public class MediaUploadData
    {
        [JsonProperty("file_token")]
        public string FileToken { get; set; }
    }
}
