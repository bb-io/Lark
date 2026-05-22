namespace Apps.Appname.Constants;

public static class PlatformTypes
{
    public const string Lark = "lark";
    public const string Feishu = "feishu";

    public static readonly HashSet<string> SupportedPlatforms =
    [
        Lark,
        Feishu
    ];
}
