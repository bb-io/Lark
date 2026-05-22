using Apps.Appname.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Appname.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "Developer API key",
            DisplayName = "Developer API key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredsNames.Platform)
                {
                    DisplayName = "Platform",
                    DataItems = new ConnectionPropertyValue[]
                    {
                        new(PlatformTypes.Lark, "Lark (International)"),
                        new(PlatformTypes.Feishu, "Feishu (China)")
                    }
                },
                new(CredsNames.AppId) { DisplayName = "Application ID"},
                new(CredsNames.AppSecret) { DisplayName="Application secret", Sensitive=true}
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        if (!values.TryGetValue(CredsNames.Platform, out var selectedPlatform) ||
            string.IsNullOrWhiteSpace(selectedPlatform))
        {
            throw new PluginMisconfigurationException("Platform is required.");
        }

        var platform = selectedPlatform.Trim().ToLowerInvariant();
        if (!PlatformTypes.SupportedPlatforms.Contains(platform))
        {
            throw new Exception($"Unsupported platform: {selectedPlatform}");
        }

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
