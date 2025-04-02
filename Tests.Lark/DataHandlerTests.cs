using Apps.Appname.Handlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Tests.Lark.Base;

namespace Tests.Lark;

[TestClass]
public class DataHandlerTests : TestBase
{
    [TestMethod]
    public async Task Dynamic_handler_works()
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
}
