using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Dtos;

public class BaseRecordDto
{
    [Display("Record ID")]
    public string RecordId { get; set; } = string.Empty;

    [Display("Fields")]
    public List<BaseRecordFieldListItemDto> Fields { get; set; } = [];
}
