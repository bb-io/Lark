using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Polling.Models;
public class RecordIdsResponse
{
    [Display("Record ID")]
    public List<RecordIdDto> RecordIds { get; set; } = [];
}

public class RecordIdDto
{
    [Display("Record ID")]
    public string RecordId { get; set; } = string.Empty;
}
