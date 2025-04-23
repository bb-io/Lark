using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class TokenResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        [JsonProperty("tenant_access_token")]
        public string TenantAccessToken { get; set; }
        public int Expire { get; set; }
    }
}
