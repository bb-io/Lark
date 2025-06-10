using Apps.Appname.Actions;
using Apps.Lark.Models.Request;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
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
            //ChatsId = "oc_912f03eb1c64f198fe78c8d54ee39dce",
            UserId = "f4c212e7",
            MessageText = "<at user_id=\"f4c212e7\"></at>, please check the document 2",
        });

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SendFile_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.SendFile(new SendFileRequest
        {
            //ChatsId = "oc_912f03eb1c64f198fe78c8d54ee39dce",
            UserId = "f4c212e7",
            FileContent = new FileReference
            {
                Name = "Test_1.xlsx",
                ContentType = "application/vnd.ms-excel"
            }
        });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task GetMessage_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.GetMessage(new GetMessageRequest
        {
            MessageId = "om_x100b4fb9d1b650a00d2364048e6b9e8"
        });

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task EditMessage_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.EditMessage(new EditMessageRequest
        {
            MessageId = "om_x100b4fb9e4b11ca40d2136b151b79c1",
            MessageText = "Hello, World! Edited"
        });

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task SearchChats_IsSucces()
    {
        var actions = new MessageActions(InvocationContext, FileManager);
        var result = await actions.SearchChats(new SearchChatsOptions { });
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));      
        Assert.IsNotNull(result);
    }
}
