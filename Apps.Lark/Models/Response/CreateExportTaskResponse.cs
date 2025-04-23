using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class CreateExportTaskResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public CreateExportTaskData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
    public class CreateExportTaskData
    {
        [JsonProperty("ticket")]
        public string Ticket { get; set; }
    }

    public class ExportTaskResultResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public ExportTaskResultData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class ExportTaskResultData
    {
        [JsonProperty("result")]
        public ExportTaskResult Result { get; set; }
    }

    public class ExportTaskResult
    {
        [JsonProperty("extra")]
        public object Extra { get; set; }

        [JsonProperty("file_extension")]
        public string FileExtension { get; set; }

        [JsonProperty("file_name")]
        public string FileName { get; set; }

        [JsonProperty("file_size")]
        public int FileSize { get; set; }

        [JsonProperty("file_token")]
        public string FileToken { get; set; }

        [JsonProperty("job_error_msg")]
        public string JobErrorMsg { get; set; }

        [JsonProperty("job_status")]
        public int JobStatus { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
