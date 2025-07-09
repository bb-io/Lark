using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class UsersResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public UsersData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class UsersData
    {
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }

        [JsonProperty("items")]
        public List<UserItem> Items { get; set; }

        [JsonProperty("page_token")]
        public string PageToken { get; set; }
    }

    public class UserItem
    {
        [JsonProperty("avatar")]
        public Avatar Avatar { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("department_ids")]
        public List<string> DepartmentIds { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("employee_no")]
        public string EmployeeNo { get; set; }

        [JsonProperty("employee_type")]
        public int EmployeeType { get; set; }

        [JsonProperty("en_name")]
        public string EnName { get; set; }

        [JsonProperty("gender")]
        public int Gender { get; set; }

        [JsonProperty("is_tenant_manager")]
        public bool IsTenantManager { get; set; }

        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        [JsonProperty("join_time")]
        public long JoinTime { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("mobile_visible")]
        public bool MobileVisible { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("open_id")]
        public string OpenId { get; set; }

        [JsonProperty("orders")]
        public List<Order> Orders { get; set; }

        [JsonProperty("status")]
        public Status Status { get; set; }

        [JsonProperty("union_id")]
        public string UnionId { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("work_station")]
        public string WorkStation { get; set; }
    }

    public class Avatar
    {
        [JsonProperty("avatar_240")]
        public string Avatar240 { get; set; }

        [JsonProperty("avatar_640")]
        public string Avatar640 { get; set; }

        [JsonProperty("avatar_72")]
        public string Avatar72 { get; set; }

        [JsonProperty("avatar_origin")]
        public string AvatarOrigin { get; set; }
    }

    public class Order
    {
        [JsonProperty("department_id")]
        public string DepartmentId { get; set; }

        [JsonProperty("department_order")]
        public int DepartmentOrder { get; set; }

        [JsonProperty("is_primary_dept")]
        public bool IsPrimaryDept { get; set; }

        [JsonProperty("user_order")]
        public int UserOrder { get; set; }
    }

    public class Status
    {
        [JsonProperty("is_activated")]
        public bool IsActivated { get; set; }

        [JsonProperty("is_exited")]
        public bool IsExited { get; set; }

        [JsonProperty("is_frozen")]
        public bool IsFrozen { get; set; }

        [JsonProperty("is_resigned")]
        public bool IsResigned { get; set; }

        [JsonProperty("is_unjoin")]
        public bool IsUnjoin { get; set; }
    }
}
