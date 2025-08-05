using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class PersonFieldResponse
    {
        [JsonProperty("person")]
        [Display("Person")]
        public PersonData Person { get; set; }
    }

    public class PersonFieldEntry
    {
        [JsonProperty("field_name")]
        [Display("Field name")]
        public string FieldName { get; set; }

        [JsonProperty("field_id")]
        [Display("Field ID")]
        public string FieldId { get; set; }

        [JsonProperty("users")]
        public List<PersonData> Users { get; set; }
    }

    public class PersonData
    {
        [JsonProperty("id")]
        [Display("User ID")]
        public string Id { get; set; }

        [JsonProperty("name")]
        [Display("Name")]
        public string Name { get; set; }

        [Display("User mention")]
        public string UserMention => string.IsNullOrEmpty(Id) ? "" : $"<at user_id=\"{Id}\">{Name}</at>";
    }
}
