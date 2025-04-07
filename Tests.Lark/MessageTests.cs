using Apps.Appname.Actions;
using Apps.Lark.Models.Request;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Lark.Base;

namespace Tests.Lark;

[TestClass]
public class MessageTests : TestBase
{
    [TestMethod]
    public async Task SendMessage_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.SendMessage(new SendMessageRequest
        {
            ChatsId = "oc_912f03eb1c64f198fe78c8d54ee39dce",
            //UserId = "f4c212e7",
            MessageText = "Hello, World!",
        });

        Console.WriteLine(result.Msg);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SendFile_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.SendFile(new SendFileRequest
        {
            ChatsId = "oc_912f03eb1c64f198fe78c8d54ee39dce",
            //UserId = "f4c212e7",
            FileContent = new FileReference
            {
                Name = "Test_1.xlsx",
                ContentType = "application/vnd.ms-excel"
            },
            FileName = "Test_1.xlsx"
        });

        Console.WriteLine(result.Msg);
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetMessage_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.GetMessage(new GetMessageRequest
        {
            MessageId = "om_7d34681776e46f45478b422410017d7b"
        });

        var items = result.Data.Items;
        foreach (var item in items)
        {
            Console.WriteLine(item.Body.Content);
        }

        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task EditMessage_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.EditMessage(new EditMessageRequest
        {
            MessageId = "om_ad0052c66a210f38e360f497ee0cc6ac",
            MessageText = "Hello, World! Edited"
        });

        Assert.IsNotNull(result);
    }
}
