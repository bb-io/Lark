using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
using Tests.Lark.Base;

namespace Tests.Lark
{
    [TestClass]
    public class SpreadsheetTests : TestBase
    {
        [TestMethod]
        public async Task CreateSpreadsheet_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var result = await actions.CreateSpreadsheet(new CreateSpreadsheetRequest
            {
                SpreadsheetName = "Test_1",
                //FolderToken = ""
            });
            Console.WriteLine(result.Msg);
            Console.WriteLine(result.Data.Spreadsheet.Url);
            Assert.IsNotNull(result);
        }

    }
}
