using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.DataSourceHandlers.Enum
{
    public class ModeTypeHandler : IStaticDataSourceItemHandler
    {
        public IEnumerable<DataSourceItem> GetData()
        {
            return EnumValues.Select(item => new DataSourceItem(item.Key, item.Value));
        }
        protected Dictionary<string, string> EnumValues => new()
        {
                {"ROW", "Row"},
                {"COLUMN", "Column"},
        };
    }
}
