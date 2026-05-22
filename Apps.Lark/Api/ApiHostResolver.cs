using Apps.Appname.Constants;

namespace Apps.Appname.Api;

public static class ApiHostResolver
{
    public static string GetBaseUrl(string? platform)
    {
        var normalizedPlatform = string.IsNullOrWhiteSpace(platform)
            ? PlatformTypes.Lark
            : platform.Trim().ToLowerInvariant();

        return normalizedPlatform switch
        {
            PlatformTypes.Feishu => "https://open.feishu.cn/open-apis",
            _ => "https://open.larksuite.com/open-apis"
        };
    }
}
