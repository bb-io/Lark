using Apps.Appname.Api;
using Apps.Lark.Webhooks.Bridge;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Lark.Webhooks.Handlers.Base
{
    public class BaseWebhookHandler : BaseInvocable, IWebhookEventHandler, IAsyncValidatableWebhookEventHandler
    {
        private readonly string _subscriptionEvent;
        private readonly string? _baseId;

        public BaseWebhookHandler(InvocationContext invocationContext, string subEvent, string? baseId=null) : base(invocationContext)
        {
            _subscriptionEvent = subEvent;
            _baseId = baseId;
        }

        public async Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
            var bridge = new BridgeService(authenticationCredentialsProviders);
            var bridgeServiceUrl = InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/');
            var payloadUrl = values["payloadUrl"];

            if (_baseId != null)
                await SubscribeToBaseEventsAsync(authenticationCredentialsProviders, _baseId);

            await bridge.SubscribeAsync(_subscriptionEvent, payloadUrl, bridgeServiceUrl);
        }

        public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders, Dictionary<string, string> values)
        {
            var bridge = new BridgeService(authenticationCredentialsProviders);
            await bridge.UnsubscribeAsync(
                _subscriptionEvent,
                values["payloadUrl"],
                InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/'));
        }

        public async Task<WebhookSubscriptionValidationResponse> ValidateSubscription(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            Dictionary<string, string> values)
        {
            if (!values.TryGetValue("payloadUrl", out var payloadUrl) || string.IsNullOrWhiteSpace(payloadUrl))
                return Invalid("The Blackbird webhook URL is missing, so the Lark subscription cannot be validated.");

            try
            {
                var bridge = new BridgeService(authenticationCredentialsProviders);
                var bridgeServiceUrl = InvocationContext.UriInfo.BridgeServiceUrl.ToString().TrimEnd('/');
                var isBridgeSubscriptionValid = await bridge.IsSubscribedAsync(
                    _subscriptionEvent,
                    payloadUrl,
                    bridgeServiceUrl);

                if (!isBridgeSubscriptionValid)
                    return Invalid("The Lark bridge registration no longer exists. Reactivate the Bird to recreate it.");

                if (_baseId != null && !await IsBaseSubscriptionActiveAsync(authenticationCredentialsProviders, _baseId))
                    return Invalid("The Lark Base event subscription is inactive. Reactivate the Bird to subscribe again.");

                return new WebhookSubscriptionValidationResponse { IsValid = true };
            }
            catch (Exception exception)
            {
                return Invalid($"Could not validate the Lark webhook subscription: {exception.Message}");
            }
        }

        private static async Task SubscribeToBaseEventsAsync(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            string baseId)
        {
            var larkClient = new LarkClient(authenticationCredentialsProviders);
            var request = new RestRequest($"drive/v1/files/{baseId}/subscribe", Method.Post)
                .AddQueryParameter("file_type", "bitable");

            await larkClient.ExecuteWithErrorHandling(request);
        }

        private static async Task<bool> IsBaseSubscriptionActiveAsync(
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
            string baseId)
        {
            var larkClient = new LarkClient(authenticationCredentialsProviders);
            var request = new RestRequest($"drive/v1/files/{baseId}/get_subscribe", Method.Get)
                .AddQueryParameter("file_type", "bitable");

            var response = await larkClient.ExecuteWithErrorHandling<JObject>(request);
            return response["data"]?["is_subscribe"]?.Value<bool?>()
                ?? response["data"]?["is_subseribe"]?.Value<bool?>()
                ?? false;
        }

        private static WebhookSubscriptionValidationResponse Invalid(string message)
            => new() { IsValid = false, Message = message };
    }
}
