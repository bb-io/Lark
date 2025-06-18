using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class FieldsResponseDto
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public FieldData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
    public class FieldData
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("items")]
        public List<FieldItem> Items { get; set; }

        [JsonProperty("page_token")]
        public string PageToken { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }

    public class FieldItem
    {
        [JsonProperty("field_id")]
        public string FieldId { get; set; }

        [JsonProperty("field_name")]
        public string FieldName { get; set; }

        [JsonProperty("is_primary")]
        public bool IsPrimary { get; set; }

        [JsonProperty("property")]
        public FieldProperty Property { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("ui_type")]
        public string UiType { get; set; }
    }

    public class FieldProperty
    {
        [JsonProperty("options")]
        public List<SelectOption> Options { get; set; }

        [JsonProperty("auto_fill")]
        public bool? AutoFill { get; set; }

        [JsonProperty("date_formatter")]
        public string DateFormatter { get; set; }
    }

    public class SelectOption
    {
        [JsonProperty("color")]
        public int Color { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
