using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
using Newtonsoft.Json;
using Tests.Lark.Base;

namespace Tests.Lark
{
    [TestClass]
    public class UserTests : TestBase
    {
        [TestMethod]
        public async Task GetUserInfo_IsSuccess()
        {
            var actions = new UserActions(InvocationContext);
            var result = await actions.GetUserInfo(new GetUserRequest
            {
                UserId = "f4c212e7"
            });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetUserInfoByEmail_IsSuccess()
        {
            var actions = new UserActions(InvocationContext);
            var result = await actions.GetUserInfoByEmail(new GetUserByEmailRequest
            {
                Email = "ariabushenko@blackbird.io"
            });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.IsNotNull(result);
        }

    }
}
