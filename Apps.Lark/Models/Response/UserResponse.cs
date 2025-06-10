using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace Apps.Lark.Models.Response
{
    public sealed class UserResponse
    {
        [JsonProperty("code")]
        public int Code { get; init; }

        [JsonProperty("data")]
        public ResponseData Data { get; init; }

        [JsonProperty("msg")]
        [Display("Message")]
        public string Msg { get; init; }
    }

    public sealed class ResponseData
    {
        [JsonProperty("user")]
        public UserDto User { get; init; }
    }

    public sealed class UserDto
    {
        [JsonProperty("avatar")]
        public AvatarDto Avatar { get; init; }

        [JsonProperty("city")]
        public string City { get; init; }

        [JsonProperty("country")]
        public string Country { get; init; }

        [JsonProperty("department_ids")]
        [Display("Departament IDs")]
        public List<string> DepartmentIds { get; init; }

        [JsonProperty("description")]
        public string Description { get; init; } 

        [JsonProperty("email")]
        public string Email { get; init; }

        [JsonProperty("employee_no")]
        [Display("Employee number")]
        public string EmployeeNumber { get; init; } 

        [JsonProperty("employee_type")]
        [Display("Employee type")]
        public int EmployeeType { get; init; }

        [JsonProperty("en_name")]
        [Display("English name")]
        public string EnglishName { get; init; }

        [JsonProperty("gender")]
        public int Gender { get; init; }

        [JsonProperty("is_tenant_manager")]
        [Display("Is tenant manager")]
        public bool IsTenantManager { get; init; }

        [JsonProperty("job_title")]
        [Display("Job title")]
        public string JobTitle { get; init; }

        [JsonProperty("join_time")]
        [Display("Join time")]
        public long JoinTime { get; init; }

        [JsonProperty("mobile")]
        public string Mobile { get; init; }

        [JsonProperty("mobile_visible")]
        [Display("Is mobile visible")]
        public bool IsMobileVisible { get; init; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("open_id")]
        [Display("Open ID")]
        public string OpenId { get; init; }

        [JsonProperty("orders")]
        public List<OrderDto> Orders { get; init; }

        [JsonProperty("status")]
        public StatusDto Status { get; init; }

        [JsonProperty("union_id")]
        [Display("Union ID")]
        public string UnionId { get; init; } 

        [JsonProperty("user_id")]
        [Display("User ID")]
        public string UserId { get; init; }

        [JsonProperty("work_station")]
        [Display("Work station")]
        public string WorkStation { get; init; }
    }

    public sealed class AvatarDto
    {
        [JsonProperty("avatar_240")]
        [Display("Avatar 240")]
        public string Avatar240 { get; init; } 

        [JsonProperty("avatar_640")]
        [Display("Avatar 640")]
        public string Avatar640 { get; init; }

        [JsonProperty("avatar_72")]
        [Display("Avatar 72")]
        public string Avatar72 { get; init; }

        [JsonProperty("avatar_origin")]
        [Display("Avatar origin")]
        public string AvatarOrigin { get; init; } 
    }

    public sealed class OrderDto
    {
        [JsonProperty("department_id")]
        [Display("Department ID")]
        public string DepartmentId { get; init; } 

        [JsonProperty("department_order")]
        [Display("Department order")]
        public int DepartmentOrder { get; init; }

        [JsonProperty("is_primary_dept")]
        [Display("Is primary department")]
        public bool IsPrimaryDepartment { get; init; }

        [JsonProperty("user_order")]
        [Display("User order")]
        public int UserOrder { get; init; }
    }

    public sealed class StatusDto
    {
        [JsonProperty("is_activated")]
        [Display("Is activated")]
        public bool IsActivated { get; init; }

        [JsonProperty("is_exited")]
        [Display("Is exited")]
        public bool IsExited { get; init; }

        [JsonProperty("is_frozen")]
        [Display("Is frozen")]
        public bool IsFrozen { get; init; }

        [JsonProperty("is_resigned")]
        [Display("Is resigned")]
        public bool IsResigned { get; init; }

        [JsonProperty("is_unjoin")]
        [Display("Is unjoined")]
        public bool IsUnjoin { get; init; }
    }
}
