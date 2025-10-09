using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Lark.Actions;

[ActionList("Users")]
public class UserActions(InvocationContext invocationContext) : Invocable(invocationContext)
{
    [Action("Get user information", Description = "Gets information about user by ID")]
    public async Task<GetUserResponse> GetUserInfo([ActionParameter] GetUserRequest getUser)
    {
        var client = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
        var request = new RestRequest($"/contact/v3/users/{getUser.UserId}?user_id_type=user_id", Method.Get);

        var response = await client.ExecuteWithErrorHandling<UserResponse>(request);

        return new GetUserResponse { UserInfo=response.Data.User };
    }

    [Action("Get user information from email", Description = "Gets information about user from email")]
    public async Task<UserInfoByEmailResponse> GetUserInfoByEmail([ActionParameter] GetUserByEmailRequest getUser)
    {
        var client = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
        var request = new RestRequest($"/contact/v3/users/batch_get_id?user_id_type=user_id", Method.Post);
        request.AddJsonBody(new
        {
            emails = new[] { getUser.Email }
        });
        var response = await client.ExecuteWithErrorHandling<UserInfoByEmail>(request);
        var userList = response.Data.UserList;

        return new UserInfoByEmailResponse { UserList = response.Data.UserList };
    }
}
