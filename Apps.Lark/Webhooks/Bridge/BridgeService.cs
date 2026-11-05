using Apps.Appname.Constants;
using Apps.Lark.Webhooks.Pyload;
using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;

namespace Apps.Lark.Webhooks.Bridge
{
    public class BridgeService
    {
        private readonly string _applicationId;

        public BridgeService(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var appIdProvider = authenticationCredentialsProviders
                                    .FirstOrDefault(x => x.KeyName == CredsNames.AppId);
            _applicationId = appIdProvider?.Value
                ?? throw new InvalidOperationException("Application ID is missing from the connection credentials.");
        }

        public async Task SubscribeAsync(string @event, string url, string bridgeServiceUrl)
        {
            var client = new RestClient($"{bridgeServiceUrl}/webhooks/lark");

            var request = new RestRequest($"/{_applicationId}/{@event}", Method.Post)
                .AddHeader("Blackbird-Token", ApplicationConstants.BlackbirdToken)
                .AddBody(url);

            var response = await client.ExecuteAsync(request);
            EnsureSuccessful(response, "subscribe the Blackbird webhook URL in the Lark bridge");
        }

        public async Task UnsubscribeAsync(string @event, string url, string bridgeServiceUrl)
        {
            var client = CreateClient(bridgeServiceUrl);
            var webhooks = await GetSubscriptionsAsync(client, @event);
            var webhook = webhooks.FirstOrDefault(w => string.Equals(w.Value, url, StringComparison.Ordinal));

            if (webhook is null) return;

            var requestDelete = new RestRequest($"/{_applicationId}/{@event}/{webhook.Id}", Method.Delete)
                .AddHeader("Blackbird-Token", ApplicationConstants.BlackbirdToken);

            var response = await client.ExecuteAsync(requestDelete);
            EnsureSuccessful(response, "unsubscribe the Blackbird webhook URL from the Lark bridge");
        }

        public async Task<bool> IsSubscribedAsync(string @event, string url, string bridgeServiceUrl)
        {
            var webhooks = await GetSubscriptionsAsync(CreateClient(bridgeServiceUrl), @event);
            return webhooks.Any(webhook => string.Equals(webhook.Value, url, StringComparison.Ordinal));
        }

        private RestClient CreateClient(string bridgeServiceUrl)
            => new($"{bridgeServiceUrl}/webhooks/lark");

        private async Task<List<BridgeGetResponse>> GetSubscriptionsAsync(RestClient client, string @event)
        {
            var request = new RestRequest($"/{_applicationId}/{@event}")
                .AddHeader("Blackbird-Token", ApplicationConstants.BlackbirdToken);

            var response = await client.ExecuteAsync<List<BridgeGetResponse>>(request);
            EnsureSuccessful(response, "get Lark bridge subscriptions");
            return response.Data ?? [];
        }

        private static void EnsureSuccessful(RestResponse response, string operation)
        {
            if (response.IsSuccessful)
                return;

            throw new InvalidOperationException(
                $"Failed to {operation}. HTTP {(int)response.StatusCode} ({response.StatusCode}). {response.ErrorMessage}");
        }
    }
}
