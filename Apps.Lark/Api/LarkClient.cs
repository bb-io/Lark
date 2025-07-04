using Apps.Appname.Constants;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Appname.Api;

public class LarkClient : BlackBirdRestClient
{
    public LarkClient(IEnumerable<AuthenticationCredentialsProvider> creds) : base(new()
    {
        BaseUrl = new Uri("https://open.larksuite.com/open-apis"),
    })
    {
        var appId = creds.First(v => v.KeyName == CredsNames.AppId).Value;
        var appSecret = creds.First(v => v.KeyName == CredsNames.AppSecret).Value;

        var token = GetToken(appId, appSecret).GetAwaiter().GetResult();
        this.AddDefaultHeader("Authorization", $"Bearer {token}");
        //this.AddDefaultHeader("Content-Type", "application/json");
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        ErrorResponse errorResponse = null;
        try
        {
            errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
            string errorMessage = $"Error {errorResponse.Code}: {errorResponse.Msg}";
            if (errorResponse.Error != null)
            {
                errorMessage += $", Details: {JsonConvert.SerializeObject(errorResponse.Error)}";
            }
            return new PluginApplicationException(errorMessage);
        }
        catch (Exception)
        {
            return new PluginApplicationException("An error occurred: " + response.ErrorException + response.StatusCode);
        }
    }
    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        var response = await ExecuteAsync(request);

        if (!response.IsSuccessStatusCode)
            throw ConfigureErrorException(response);

        return response;
    }

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        var response = await ExecuteWithErrorHandling(request);
        return JsonConvert.DeserializeObject<T>(response.Content, JsonSettings);
    }

    public async Task<string> GetToken(string appId, string appSecret)
    {
        var request = new RestRequest("/auth/v3/tenant_access_token/internal", Method.Post);


        request.AddBody(new { app_id = appId, app_secret = appSecret });

        TokenResponse response;
        try
        {
            response = this.ExecuteWithErrorHandling<TokenResponse>(request)
                           .GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException("Error logging in", ex);
        }

        return response.TenantAccessToken;
    }
}