using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Response;
using Apps.Lark.Webhooks.Handlers;
using Apps.Lark.Webhooks.Payload;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Webhooks
{
    [WebhookList]
    public class WebhookList: Invocable
    {
        public WebhookList(InvocationContext invocationContext):base(invocationContext)
        {
        }


        [Webhook("On message received", typeof(MessageReceivedHandler), Description = "This event is triggered when the bot receives a message sent by a user.")]
        public async Task<WebhookResponse<MessageReceiveEvent>> ChannelMessage(WebhookRequest webhookRequest)
        {
            var payload =
                JsonConvert.DeserializeObject<BasePayload<MessageReceiveEvent>>(webhookRequest.Body.ToString());

            if (payload == null)
                throw new Exception("No serializable payload was found in incoming request.");

            return new WebhookResponse<MessageReceiveEvent>
            {
                HttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK),
                Result = payload.Event,
                ReceivedWebhookRequestType = WebhookRequestType.Default
            };

        }
    }
}
