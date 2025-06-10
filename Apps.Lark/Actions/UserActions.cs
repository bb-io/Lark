using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Lark.Actions
{
    [ActionList]
    public class UserActions(InvocationContext invocationContext) : Invocable(invocationContext)
    {
        [Action("Get user information", Description = "Gets information about user")]
        public async Task<GetUserResponse> GetUserInfo([ActionParameter] GetUserRequest getUser)
        {
            var client = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new RestRequest($"/contact/v3/users/{getUser.UserId}?user_id_type=user_id", Method.Get);

            var response = await client.ExecuteWithErrorHandling<UserResponse>(request);

            return new GetUserResponse { UserInfo=response.Data.User };
        }
    }
}
