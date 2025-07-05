using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Polling.Models;
public class RecordCreatedRequest
{
    [Display("Title of the field that stores creation date")]
    public string FieldName { get; set; } = string.Empty;
}
