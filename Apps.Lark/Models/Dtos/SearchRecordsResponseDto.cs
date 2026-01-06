using Apps.Lark.Models.Response;

namespace Apps.Lark.Models.Dtos
{
    public class SearchRecordsResponseDto
    {
        public int Code { get; set; }
        public string? Message { get; set; }
        public SearchRecordsDataDto? Data { get; set; }
    }

    public class SearchRecordsDataDto
    {
        public List<RecordItemDto>? Items { get; set; }
        public bool HasMore { get; set; }
        public string? PageToken { get; set; }
    }
}
