using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
using Blackbird.Applications.Sdk.Common.Files;
using Tests.Lark.Base;

namespace Tests.Lark
{
    [TestClass]
    public class BaseTableTests : TestBase
    {
        [TestMethod]
        public async Task SearchBaseTables_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.SearchBaseTables(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecord_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetRecord(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" }, new GetBaseRecord { RowIndex = 0 });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task UpdateBaseRecord_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.UpdateRecord(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
                new UpdateRecordRequest
                {
                    FieldName = "Multi option choice",
                    //NewValue="Hello my new value from upate action 2"
                    //NewDateValue= DateTime.UtcNow.AddDays(2),
                    NewValues = new List<string> { "Option 12", "Option 21" },
                    //Attachment = new FileReference { Name = "Test3.png" }
                },
                new GetBaseRecord { RowIndex = 6 });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordPersonTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetPersonEntry(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
                new GetBaseRecord { RowIndex = 0 });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordDateTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetDateEntries(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
                new GetBaseRecord { RowIndex = 0 });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordFilesTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.DownloadAttachments(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
                new GetBaseRecord { RowIndex = 0 });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }


        [TestMethod]
        public async Task GetBaseTableUsedRange_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetBaseRecords(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        //InsertBaseTableRow

        [TestMethod]
        public async Task InsertBaseTableRow_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            await action.InsertBaseTableRow(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" });

            Assert.IsTrue(true);
        }
    }
}
