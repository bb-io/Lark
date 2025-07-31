namespace Apps.Lark.Polling.Models;

public class BaseTableRecordChangedMemory
{
    public DateTime LastPollingTime { get; set; }

    public string LastObservedFieldValue { get; set; } = string.Empty;
}
