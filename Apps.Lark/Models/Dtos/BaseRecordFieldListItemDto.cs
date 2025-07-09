using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Dtos;
public class BaseRecordFieldListItemDto
{
    [Display("Field ID")]
    [JsonProperty("Field ID")] // This is used to match the display name in the UI
    public string FieldId { get; set; } = string.Empty;

    [Display("Field name")]
    [JsonProperty("Field name")] // This is used to match the display name in the UI
    public string FieldName { get; set; } = string.Empty;

    [Display("Field type")]
    [JsonProperty("Field type")] // This is used to match the display name in the UI
    public string FieldType { get; set; } = string.Empty;

    [Display("Field value")]
    [JsonProperty("Field value")] // This is used to match the display name in the UI
    public string FieldValue { get; set; } = string.Empty;
}
