using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Dtos;
public class BaseRecordFieldListItemDto
{
    [Display("Field ID")]
    public string FieldId { get; set; } = string.Empty;

    [Display("Field name")]
    public string FieldName { get; set; } = string.Empty;

    [Display("Field type")]
    public string FieldType { get; set; } = string.Empty;

    [Display("Field value")]
    public string FieldValue { get; set; } = string.Empty;
}
