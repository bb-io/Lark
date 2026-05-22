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
            Name = PlatformTypes.Lark,
            DisplayName = "Lark (International)",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.AppId) { DisplayName = "Application ID"},
                new(CredsNames.AppSecret) { DisplayName="Application secret", Sensitive=true}
            }
        },
        new()
        {
            Name = PlatformTypes.Feishu,
            DisplayName = "Feishu (China)",
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
        var platform = values.TryGetValue(nameof(ConnectionPropertyGroup), out var selectedGroup) &&
            PlatformTypes.SupportedPlatforms.Contains(selectedGroup)
            ? selectedGroup
            : PlatformTypes.Lark;

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

        yield return new AuthenticationCredentialsProvider(
            CredsNames.Platform,
            platform
        );
    }
}
