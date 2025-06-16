using Apps.Appname.Api;
using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
using Newtonsoft.Json;
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
                SpreadsheetName = "Test_new21",
                FolderToken = "BjPQfjg2mlp76MdwnmPjX4jrpRh"
            });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task FindCells_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var result = await actions.FindCells(new FindCellsRequest
            {
                Query = "6",
                Range = "C1:G17"
            }, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });
            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task AddRows_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var result = await actions.AddRowsOrColumns(new AddRowsOrColumnsRequest
            {
                Length = 1,
                InsertMode = "ROW"
            }, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });
            Console.WriteLine(result.Msg);
            Assert.IsNotNull(result);
        }
        [TestMethod]
        public async Task AddColumns_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var result = await actions.AddRowsOrColumns(new AddRowsOrColumnsRequest
            {
                Length = 1,
                InsertMode = "COLUMN"
            }, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
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

                StartIndex = 1,
                EndIndex = 30,
                InsertMode = "ROW"
            }, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });
            Console.WriteLine(result.Msg);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UpdateRows_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var updateRequest = new UpdateRowsRequest
            {
                Range = "C13:F17",
                Values = new List<string>
            {
                "Edit5, 1, http://www.xx.com",
                "Edit25, 12, 18, me@HelloWorld.1com",
                "Edit5, 13, 12, 6",
                "Edit5, 14, 6, @Jack"
            }
            };

            var result = await actions.AddOrUpdateRows(updateRequest, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UpdateCell_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var updateRequest = new UpdateSheetCell
            {
                SheetCell = "C1",
                Value = "Edited from unit test call"
            };

            var result = await actions.UpdateSheetCell(updateRequest, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d"
            });
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task UpdateColumns_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var updateRequest = new UpdateRowsRequest
            {
                Range = "B1:C4",
                Values = new List<string>
            {
                "Edited by Artem",
                "Edited by Artem",
                "Edited by Artem",
                "Edited by Artem"
            }
            };

            var result = await actions.AddOrUpdateRows(updateRequest, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });
            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task InsertRows_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var updateRequest = new UpdateRowsRequest
            {
                Range = "C10:F14",
                Values = new List<string>
            {
                "Edit inserted, 1, http://www.xx.com",
                "Edit inserted, 12, 18, me@HelloWorld.1com",
                "Edit inserted, 13, 12, 6",
                "Edit inserted, 14, 6, @Jack"
            }
            };

            var result = await actions.InsertRows(updateRequest, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });
            Console.WriteLine(result.Msg);
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task GetRangeCellsValues_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var updateRequest = new GetRangeCellsValuesRequest
            {
                Range = "C1:D14"
            };

            var result = await actions.GetRangeCellsValues(updateRequest, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task CreateExportTask_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var client = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var sheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh";

            var result = await actions.CreateExportTask(client, sheetToken);
            Console.WriteLine(result);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetExportTaskResult_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var client = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var sheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh";
            var ticket = "7490603441450926114";

            var result = await actions.GetExportTaskResult(client, ticket, sheetToken);
            Console.WriteLine(result.FileToken);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DownloadFile_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var client = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);
            var fileToken = "HvW7b58Dro9ptWx7GCMj8vRlpxe";

            var result = await actions.DownloadFile(client, fileToken, "TestSpreadsheetA", "xlsx");
            //Console.WriteLine($"{result.Name} - {result.Size}");
            Assert.IsNotNull(result);
        }


        [TestMethod]
        public async Task GetCellValue_IsSucces()
        {
            var actions = new SpreadsheetActions(InvocationContext, FileManager);
            var updateRequest = new GetSheetCell
            {
               Cell= "C1"
            };

            var result = await actions.GetSheetCell(updateRequest, new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            });

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(result);
        }
    }
}
