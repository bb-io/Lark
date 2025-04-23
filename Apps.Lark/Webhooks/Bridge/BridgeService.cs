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
            _applicationId = appIdProvider.Value;

        }


        public void Subscribe(string @event, string url, string bridgeServiceUrl)
        {
            var client = new RestClient($"{bridgeServiceUrl}/webhooks/lark");

            var request = new RestRequest($"/{_applicationId}/{@event}", Method.Post)
                .AddHeader("Blackbird-Token", ApplicationConstants.BlackbirdToken)
                .AddBody(url);

            client.Execute(request);
        }

        public void Unsubscribe(string @event, string url, string bridgeServiceUrl)
        {
            var client = new RestClient($"{bridgeServiceUrl}/webhooks/lark");

            var requestGet = new RestRequest($"/{_applicationId}/{@event}")
                .AddHeader("Blackbird-Token", ApplicationConstants.BlackbirdToken);

            var webhooks = client.Get<List<BridgeGetResponse>>(requestGet);
            var webhook = webhooks.FirstOrDefault(w => w.Value == url);

            if (webhook is null) return;

            var requestDelete = new RestRequest($"/{_applicationId}/{@event}/{webhook.Id}", Method.Delete)
                .AddHeader("Blackbird-Token", ApplicationConstants.BlackbirdToken);

            client.Delete(requestDelete);
        }
    }
}
