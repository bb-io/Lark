using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
using Apps.Lark.Polling.Models;
using Apps.Lark.Webhooks;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Webhooks;
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
            var response = await action.GetRecord(
                new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
                new GetBaseRecord { RecordID = "recuOXSfSwQlV8" });

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
                    FieldName = "Custom text column name",
                    NewValue="Hello my new value from upate action locally "
                    //NewDateValue= DateTime.UtcNow.AddDays(2),
                    //NewValues = new List<string> { "Option 12", "Option 21345435" },
                    //Attachment = new FileReference { Name = "Test3.png" }
                },
                new GetBaseRecord { RecordID = "recuOXSfSwQlV8" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordPersonTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetPersonEntry(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recQXFIkrm" },
                new GetPersonFieldRequest { FieldId= "fldqncxBMn" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordDateTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetDateEntries(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recaqVFKCW" },
                new GetDateFieldRequest { FieldId= "fld3o9NPaH" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }


        [TestMethod]
        public async Task GetBaseRecordTextTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            //var response = await action.GetTextEntry(new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            //    new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" },
            //    new GetBaseRecord { RecordID = "recuQsfE1GO90j" },
            //    new GetTextFieldRequest { FieldId= "fldKO35rlm" });

            var response = await action.GetTextEntry(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
               new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
               new GetBaseRecord { RecordID = "recaqVFKCW" },
               new GetTextFieldRequest { FieldId = "fldBAPISc0" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetMultiOptionValueFromRecord_IsSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetMultiOptionValueFromRecord(
                new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recaqVFKCW" },
                new GetFieldRequest { FieldId = "fldlvpfJ7u" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordNumberTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetNumberEntry(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recaqVFKCW" },
                new GetNumberFieldRequest { FieldId = "fldJUxetZw" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }
      
        [TestMethod]
        public async Task GetBaseRecordFilesTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.DownloadAttachments(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recaqVFKCW" },
                new GetDownloadFieldRequest { FieldId= "fldsZurxhF" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseTableUsedRange_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetBaseRecords(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

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
