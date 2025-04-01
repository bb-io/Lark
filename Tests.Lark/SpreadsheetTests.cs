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


        [TestMethod]
        public async Task FindCells_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var result = await actions.FindCells(new FindCellsRequest
            {
                SpreadsheetToken = "NwdisvdflhiqgDtebS2jLF6Rp2c",
                SheetId = "18cf64",
                Query = "Test",
                Range = "A1:A1"
            });
            Console.WriteLine(result.Msg);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task AddRowsOrColumns_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var result = await actions.AddRowsOrColumns(new AddRowsOrColumnsRequest
            {
                SpreadsheetToken = "NwdisvdflhiqgDtebS2jLF6Rp2c",
                SheetId = "18cf64",
                Length = 1,
                InsertMode = "ROW"
            });
            Console.WriteLine(result.Msg);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteRowsOrColumns_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var result = await actions.DeleteRowsOrColumns(new DeleteRowsOrColumnsRequest
            {
                SpreadsheetToken = "NwdisvdflhiqgDtebS2jLF6Rp2c",
                SheetId = "18cf64",
                StartIndex=1,
                EndIndex=3,
                InsertMode = "ROW"
            });
            Console.WriteLine(result.Msg);
            Assert.IsNotNull(result);
        }
    }
}
