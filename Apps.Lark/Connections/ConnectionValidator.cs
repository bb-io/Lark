using Apps.Appname.Api;
using Apps.Appname.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Appname.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new LarkClient(authenticationCredentialsProviders);
            var appId = authenticationCredentialsProviders.First(v => v.KeyName == CredsNames.AppId).Value;

            var request = new RestRequest($"/application/v6/applications/{appId}", Method.Get);
            request.AddParameter("lang", "en_us");

            var response = await client.ExecuteAsync(request, cancellationToken);

            return new()
            {
                IsValid = response.IsSuccessful,
            };
        }
        catch (Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }

    }
}