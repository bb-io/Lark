using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
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
                new BaseTableRequest { TableId= "tblORLQK2OUtTZ9p" }, new GetBaseRecord { RowIndex= 0 });

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
                new UpdateRecordRequest { FieldName= "Custom text column name",
                NewValue="Hello my new value from upate action 2"},
                new GetBaseRecord { RowIndex = 0 });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }
    }
}
