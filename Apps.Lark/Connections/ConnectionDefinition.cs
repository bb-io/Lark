using Apps.Appname.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Appname.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Developer API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.AppId) { DisplayName = "Application ID"},
                new(CredsNames.AppSecret) { DisplayName="Application secret", Sensitive=true}
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {

        var appId = values.First(v => v.Key == CredsNames.AppId);
        yield return new AuthenticationCredentialsProvider(
            appId.Key,
            appId.Value
        );

        var appSecret = values.First(v => v.Key == CredsNames.AppSecret);
        yield return new AuthenticationCredentialsProvider(
            appSecret.Key,
            appSecret.Value
        );
    }
}