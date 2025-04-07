using Apps.Lark.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Apps.Lark.Webhooks.Payload
{
    public class MessageReceiveEvent
    {
        [JsonProperty("sender")]
        public Sender Sender { get; set; }

        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public class Sender
    {
        [JsonProperty("sender_id")]
        public SenderId SenderId { get; set; }

        [JsonProperty("sender_type")]
        public string SenderType { get; set; }

        [JsonProperty("tenant_key")]
        public string TenantKey { get; set; }
    }

    public class SenderId
    {
        [JsonProperty("union_id")]
        public string UnionId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("open_id")]
        public string OpenId { get; set; }
    }

    public class Message
    {
        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("root_id")]
        public string RootId { get; set; }

        [JsonProperty("parent_id")]
        public string ParentId { get; set; }

        [JsonProperty("create_time")]
        public long CreateTime { get; set; }

        [JsonProperty("update_time")]
        public long UpdateTime { get; set; }

        [JsonProperty("chat_id")]
        public string ChatId { get; set; }

        [JsonProperty("thread_id")]
        public string ThreadId { get; set; }

        [JsonProperty("chat_type")]
        public string ChatType { get; set; }

        [JsonProperty("message_type")]
        public string MessageType { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("mentions")]
        public List<Mention> Mentions { get; set; }

        [JsonProperty("user_agent")]
        public string UserAgent { get; set; }
    }

     public class Mention
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("id")]
        public SenderId Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("tenant_key")]
        public string TenantKey { get; set; }
    }
}
