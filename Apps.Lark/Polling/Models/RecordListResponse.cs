using Apps.Lark.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Lark.Polling.Models;
public class RecordListResponse
{
    [Display("Base ID")]
    public string BaseId { get; set; } = string.Empty;

    [Display("Table ID")]
    public string TableId { get; set; } = string.Empty;

    [Display("Records")]
    public IEnumerable<BaseRecordDto> Records { get; set; } = [];

    [Display("Records in JSON files")]
    public IEnumerable<FileReference> RecordsJson { get; set; } = [];
}
