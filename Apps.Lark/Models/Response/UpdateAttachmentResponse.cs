using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Response
{
    public class UpdateAttachmentResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public Attach Data { get; set; }
    }
    public class Attach
    {
        [JsonProperty("file_token")]
        public string FileToken { get; set; }
    }
}
