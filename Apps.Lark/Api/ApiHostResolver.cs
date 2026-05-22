using Apps.Appname.Constants;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Appname.Api;

public static class ApiHostResolver
{
    public static string GetBaseUrl(string? platform)
    {
        if (string.IsNullOrWhiteSpace(platform))
        {
            throw new PluginMisconfigurationException($"Platform is required. {nameof(platform)}");
        }

        var normalizedPlatform = platform.Trim().ToLowerInvariant();

        return normalizedPlatform switch
        {
            PlatformTypes.Feishu => "https://open.feishu.cn/open-apis",
            PlatformTypes.Lark => "https://open.larksuite.com/open-apis",
            _ => throw new PluginMisconfigurationException($"Unsupported platform: {platform}")
        };
    }
}
