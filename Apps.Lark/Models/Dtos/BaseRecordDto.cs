using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Dtos;

public class BaseRecordDto
{
    [Display("Record ID")]
    [JsonProperty("Record ID")] // This is used to match the display name in the UI
    public string RecordId { get; set; } = string.Empty;

    [Display("Fields")]
    [JsonProperty("Fields")] // This is used to match the display name in the UI
    public List<BaseRecordFieldListItemDto> Fields { get; set; } = [];
}
