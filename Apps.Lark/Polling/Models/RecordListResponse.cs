using Apps.Lark.Models.Dtos;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Polling.Models;
public class RecordListResponse
{
    [Display("Base ID")]
    public string BaseId { get; set; } = string.Empty;

    [Display("Table ID")]
    public string TableId { get; set; } = string.Empty;

    [Display("Records")]
    public List<BaseRecordDto> Records { get; set; } = [];
}
