using Apps.Appname.Api;
using Apps.Appname.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;
using System.Net;

namespace Apps.Appname.Connections;

public class ConnectionValidator : IConnectionValidator
{
    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        try
        {
            var appId = authenticationCredentialsProviders.First(v => v.KeyName == CredsNames.AppId).Value;
            var appSecret = authenticationCredentialsProviders.First(v => v.KeyName == CredsNames.AppSecret).Value;

            using var client = new LarkClient(authenticationCredentialsProviders);
            var request = new RestRequest("/auth/v3/tenant_access_token/internal", Method.Post);
            request.AddBody(new { app_id = appId, app_secret = appSecret });

            var response = await client.ExecuteAsync(request, cancellationToken);

            if (response.StatusCode is HttpStatusCode.Unauthorized)
                throw new HttpRequestException(
                    $"{response.StatusCode}: {response.Content ?? response.ErrorMessage}",
                    null,
                    HttpStatusCode.Unauthorized);

            if (response.StatusCode is HttpStatusCode.Forbidden)
                throw new HttpRequestException(
                    $"{response.StatusCode}: {response.Content ?? response.ErrorMessage}",
                    null,
                    HttpStatusCode.Forbidden);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.Unauthorized)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode is HttpStatusCode.Forbidden)
        {
            return new()
            {
                IsValid = false,
                Message = ex.Message
            };
        }
        catch (Exception)
        {
            return new() { IsValid = true };
        }

        return new() { IsValid = true };
    }
}
