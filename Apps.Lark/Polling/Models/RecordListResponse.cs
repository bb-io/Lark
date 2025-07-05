using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Polling.Models;
public class RecordListResponse
{
    [Display("Record ID")]
    public List<RecordListItemDto> RecordIds { get; set; } = [];
}

public class RecordListItemDto
{
    [Display("Record ID")]
    public string RecordId { get; set; } = string.Empty;

    [Display("Database ID")]
    public string DatabaseId { get; set; } = string.Empty;

    [Display("Table ID")]
    public string TableId { get; set; } = string.Empty;
}
