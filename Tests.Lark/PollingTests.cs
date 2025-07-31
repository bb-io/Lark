using Apps.Lark.Models.Request;
using Apps.Lark.Polling;
using Apps.Lark.Polling.Models;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Lark.Base;

namespace Tests.Lark
{
    [TestClass]
    public class PollingTests : TestBase
    {
        [TestMethod]
        public async Task NewRowAddedMemory_IsSucces()
        {
            var actions = new PollingList(InvocationContext, FileManager);
            var spreadsheet = new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            };
            var pollingRequest = new PollingEventRequest<NewRowAddedMemory>
            {
                Memory = new NewRowAddedMemory
                {
                    LastRowCount = 19,
                    LastPollingTime = DateTime.UtcNow.AddDays(-1),
                    Triggered = false
                }
            };
            var result = await actions.OnNewRowsAdded(pollingRequest, spreadsheet);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task BaseTableNewRowAddedMemory_IsSucces()
        {
            var actions = new PollingList(InvocationContext, FileManager);
            var pollingRequest = new PollingEventRequest<DateTimeMemory>
            {
                Memory = new DateTimeMemory
                {
                    LastPollingTime = DateTime.UtcNow.AddDays(-1),
                }
            };
            var result = await actions.OnNewRowsAddedToBaseTable(
                pollingRequest,
                new BaseRequest { AppId= "U20EbzMzSaRz3psPAsBlqNEOgZd" },
                new BaseTableRequest { TableId= "tblmJb3RMxHfMFbd" }
            );
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task OnBaseTableRecordUpdated_works()
        {
            var baseRequest = new BaseRequest { AppId = "BqWJbD6KnaJpaMsj1JZjwekIpqx" };
            var baseTableRequest = new BaseTableRequest { TableId = "tblY6LBTDbElZ0l4" };
            var recordId = "recoXweOWG";
            var fieldId = "fldJje4MEe";
            var fieldValue = "O2: Optimize product features";
            var memory = new BaseTableRecordChangedMemory
            {
                LastPollingTime = DateTime.UtcNow.AddDays(-20),
                LastObservedFieldValue = string.Empty, // "O2: Optimize product features"
            };

            var actions = new PollingList(InvocationContext, FileManager);
            var result = await actions.OnBaseTableRecordUpdated(
                new() { Memory = memory },
                baseRequest,
                baseTableRequest,
                recordId,
                fieldId,
                fieldValue);

            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
            Assert.IsNotNull(result);
        }
    }
}
