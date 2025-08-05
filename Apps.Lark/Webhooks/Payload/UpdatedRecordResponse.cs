using Apps.Lark.Models.Dtos;
using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Webhooks.Payload
{
    public class UpdatedRecordResponse
    {
        [Display("Base ID")]
        public string BaseId { get; set; } = string.Empty;

        [Display("Table ID")]
        public string TableId { get; set; } = string.Empty;

        [Display("Record ID")]
        [JsonProperty("Record ID")]
        public string RecordId { get; set; } = string.Empty;

        [Display("Update time")]
        public DateTime UpdateTime { get; set; }

        [Display("Before fields")]
        [JsonProperty("Before fields")]
        public List<BaseRecordFieldListItemDto> BeforeFields { get; set; } = [];

        [Display("After fields")]
        [JsonProperty("After fields")]
        public List<BaseRecordFieldListItemDto> AfterFields { get; set; } = [];
    }
}
