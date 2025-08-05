using Apps.Lark.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Polling.Models;
public class RecordResponse
{
    [Display("Base ID")]
    public string BaseId { get; set; } = string.Empty;

    [Display("Table ID")]
    public string TableId { get; set; } = string.Empty;

    [Display("Record ID")]
    [JsonProperty("Record ID")] // This is used to match the display name in the UI
    public string RecordId { get; set; } = string.Empty;

    [Display("Fields")]
    [JsonProperty("Fields")] // This is used to match the display name in the UI
    public List<BaseRecordFieldListItemDto> Fields { get; set; } = [];

    [Display("Update time")]
    public DateTime UpdateTime { get; set; }
}
