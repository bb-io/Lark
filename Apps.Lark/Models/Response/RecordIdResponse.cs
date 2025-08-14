using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response;

public class RecordIdResponse
{
    [Display("Record ID")]
    public string RecordId { get; set; } = string.Empty;
}
