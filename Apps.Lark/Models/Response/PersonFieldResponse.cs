using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class PersonFieldResponse
    {
        [JsonProperty("person_fields")]
        [Display("Person fields")]
        public List<PersonFieldEntry> PersonFields { get; set; }
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
        public string Id { get; set; }

        [Display("User Mention")]
        public string? UserMention => $"<at user_id=\"{Id}\">{Name}</at>";

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("en_name")]
        [Display("English name")]
        public string EnName { get; set; }

        [JsonProperty("avatar_url")]
        [Display("Avatar URL")]
        public string AvatarUrl { get; set; }
    }
}
