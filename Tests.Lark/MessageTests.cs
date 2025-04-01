using Apps.Appname.Actions;
using Apps.Lark.Models.Request;
using Tests.Lark.Base;

namespace Tests.Lark;

[TestClass]
public class MessageTests : TestBase
{
    [TestMethod]
    public async Task SendMessage_IsSucces()
    {
        var actions = new MessageActions(InvocationContext);
        var result = await actions.SendMessage(new SendMessageRequest
        {
            ReceiveIdType = "user_id",
            MessageText = "Hello, World!",
            ReceiveId = "f4c212e7"
        });

        Console.WriteLine(result.Msg);
        Assert.IsNotNull(result);
    }
}
