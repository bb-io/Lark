using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class UploadFileResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public DataFile Data { get; set; }
    }
    public class DataFile
    {
        [JsonProperty("file_key")]
        public string FileKey { get; set; }
    }
}