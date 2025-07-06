using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Lark.Models.Dtos;

public class BaseTableSchemaApiResponseDto
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("data")]
    public BaseTableSchemaApiDataDto Data { get; set; } = new();
}

public class BaseTableSchemaApiDataDto
{
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("items")]
    public List<BaseTableSchemaApiItemDto> Items { get; set; } = [];
}

public class BaseTableSchemaApiItemDto
{
    [JsonProperty("field_id")]
    public string FieldId { get; set; } = string.Empty;

    [JsonProperty("field_name")]
    public string FieldName { get; set; } = string.Empty;

    [JsonProperty("type")]
    public int FieldTypeId { get; set; }

    [JsonProperty("ui_type")]
    public string FieldTypeName { get; set; } = string.Empty;

    [JsonProperty("is_primary")]
    public bool IsPrimary { get; set; }

    [JsonProperty("property")]
    public JObject? Property { get; set; }
}
