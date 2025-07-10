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
    public async Task BaseTableDataHandler_IsSuccess()
    {

        var handler = new BaseTableDataSourceHandler(InvocationContext, new BaseRequest { AppId= "Oacjbnzg3aMyAXsLgK5jR21Op0b" });

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

        var handler = new BaseTableFieldDataSourceHandler(InvocationContext, new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            new BaseTableRequest { TableId= "tblzSbOM8CQupYfE" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTableTextFieldDataHandler_IsSuccess()
    {
        var handler = new BaseTableTextFieldIdDataSourceHandler(InvocationContext, new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTablePersonFieldDataHandler_IsSuccess()
    {
        var handler = new BaseTablePersonFieldIdDataSourceHandler(InvocationContext, new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTableDateFieldDataHandler_IsSuccess()
    {
        var handler = new BaseTableDateFieldIdDataSourceHandler(InvocationContext, new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTableMultipleFieldDataHandler_IsSuccess()
    {
        var handler = new BaseTableMultipleFieldIdDataSourceHandler(InvocationContext, new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTableNumberFieldDataHandler_IsSuccess()
    {
        var handler = new BaseTableNumberFieldIdDataSourceHandler(InvocationContext, new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

    [TestMethod]
    public async Task BaseTableAttachmentFieldDataHandler_IsSuccess()
    {
        var handler = new BaseTableAttachmentFieldIdDataSourceHandler(InvocationContext, new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" });

        var result = await handler.GetDataAsync(new DataSourceContext { }, CancellationToken.None);

        Console.WriteLine($"Total: {result.Count()}");
        foreach (var item in result)
        {
            Console.WriteLine($"{item.Value}: {item.Key}");
        }

        Assert.IsTrue(result.Count() > 0);
    }

}



