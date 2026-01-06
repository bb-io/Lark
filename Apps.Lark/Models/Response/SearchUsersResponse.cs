namespace Apps.Lark.Models.Response
{
    public class SearchUsersResponse
    {
        public int Code { get; set; }
        public string? Msg { get; set; }
        public SearchUsersData? Data { get; set; }
    }
    public class SearchUsersData
    {
        public List<SearchUserItem> Users { get; set; } = new();
    }
    public class SearchUserItem
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
    }
}
