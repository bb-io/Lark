using Apps.Appname.Handlers;
using Apps.Lark.DataSourceHandlers;
using Apps.Lark.Models.Request;
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
    public async Task SpreadsheetHandler_IsSuccess()
    {
        var handler = new SpreadsheetDataSourceHandler(InvocationContext);

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
        var input = new SpreadsheetsRequest { SpreadsheetToken= "GFMMsfFV4huQxIt6Qanj8IvdpSh" };

        var handler = new SheetDataSourceHandler(InvocationContext, input);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task ChatDataHandler_IsSuccess()
    {

        var handler = new ChatDataSourceHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task UsersDataHandler_IsSuccess()
    {

        var handler = new UsersDataSourceHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }


    [TestMethod]
    public async Task BaseDataHandler_IsSuccess()
    {

        var handler = new BaseDataSourceHandler(InvocationContext);

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTableDataHandler_IsSuccess()
    {

        var handler = new BaseTableDataSourceHandler(InvocationContext, new BaseRequest { AppId= "MXjZb5uHvahFiMs5mUvjIzC9pxf" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTableFieldDataHandler_IsSuccess()
    {

        var handler = new BaseTableFieldDataSourceHandler(InvocationContext, new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
            new BaseTableRequest { TableId= "tblORLQK2OUtTZ9p" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

}



