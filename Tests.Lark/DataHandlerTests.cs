using Apps.Appname.Handlers;
using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Lark.Base;

namespace Tests.Lark;

[TestClass]
public class DataHandlerTests : TestBase
{
    [TestMethod]
    public async Task FolderHandler_IsSuccess()
    {
        var handler = new FolderDataSourceHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task SheetHandler_IsSuccess()
    {
        var handler = new SheetDataSourceHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }
}
