using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Appname.Handlers.Static;
public class ReceiveIdTypeHandler : IStaticDataSourceItemHandler
{
    public IEnumerable<DataSourceItem> GetData()
    {
        return EnumValues.Select(item => new DataSourceItem(item.Key, item.Value));
    }
    protected Dictionary<string, string> EnumValues => new()
    {
            {"open_id", "Open ID"},
            {"user_id", "User ID"},
            {"union_id", "Union ID"},
            {"email", "Email"},
            {"chat_id", "Chat ID"}
    };
}
