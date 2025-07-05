using Newtonsoft.Json;

namespace Apps.Lark.Polling.Models;

public class RecordsSearchApiResponseDto
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("data")]
    public RecordsSearchDataDto Data { get; set; } = new();
}

public class RecordsSearchDataDto
{
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("items")]
    public List<RecordsSearchItemDto> Items { get; set; } = [];
}

public class RecordsSearchItemDto
{
    [JsonProperty("fields")]
    public Dictionary<string, string> Fields { get; set; } = [];

    [JsonProperty("record_id")]
    public string RecordId { get; set; } = string.Empty;
}
