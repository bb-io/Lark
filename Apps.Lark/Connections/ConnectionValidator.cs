using Apps.Appname.Api;
using Apps.Appname.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Appname.Connections;

public class ConnectionValidator: IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = new LarkClient(authenticationCredentialsProviders);

            var appId = authenticationCredentialsProviders.First(v => v.KeyName == CredsNames.AppId).Value;
            var appSecret = authenticationCredentialsProviders.First(v => v.KeyName == CredsNames.AppSecret).Value;

            await client.GetToken(appId, appSecret);


            return new()
            {
                IsValid = true
            };
        } catch(Exception ex)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }

    }
}