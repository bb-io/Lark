using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class BaseRequest
    {
        [Display("Base ID")]
        [DataSource(typeof(BaseDataSourceHandler))]
        public string AppId { get; set; }
    }
}
