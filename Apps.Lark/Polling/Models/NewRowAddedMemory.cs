namespace Apps.Lark.Polling.Models
{
    public class NewRowAddedMemory
    {
        public DateTime? LastPollingTime { get; set; }

        public bool Triggered { get; set; }

        public int LastRowCount { get; set; }
    }
}
