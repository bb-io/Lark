using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Apps.Lark.Models.Response
{
    public class SendMessageResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("msg")]
        [Display("Message")]
        public string Msg { get; set; }
    }
    public class Data
    {
        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("chat_id")]
        [Display("Chat ID")]
        public string ChatId { get; set; }

        [JsonProperty("create_time")]
        [Display("Create time")]
        public string CreateTime { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("message_id")]
        [Display("Message ID")]
        public string MessageId { get; set; }

        [JsonProperty("msg_type")]
        [Display("Message type")]
        public string MsgType { get; set; }

        [JsonProperty("sender")]
        public Sender Sender { get; set; }

        [JsonProperty("update_time")]
        [Display("Update time")]
        public string UpdateTime { get; set; }

        [JsonProperty("updated")]
        public bool Updated { get; set; }
    }

    public class Body
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }

    public class Sender
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("id_type")]
        [Display("ID type")]
        public string IdType { get; set; }

        [JsonProperty("sender_type")]
        [Display("Sender type")]
        public string SenderType { get; set; }

        [JsonProperty("tenant_key")]
        [Display("Tenant key")]
        public string TenantKey { get; set; }
    }
}
