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
            var actions = new PollingList(InvocationContext);
            var spreadsheet = new SpreadsheetsRequest
            {
                SpreadsheetToken = "GFMMsfFV4huQxIt6Qanj8IvdpSh",
                SheetId = "a8685d",
            };
            var pollingRequest = new PollingEventRequest<NewRowAddedMemory>
            {
                Memory = new NewRowAddedMemory
                {
                    LastRowCount = 14,
                    LastPollingTime = DateTime.UtcNow.AddDays(-1),
                    Triggered = false
                }
            };
            var result = await actions.OnNewRowsAdded(pollingRequest, spreadsheet);
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented));
            Assert.IsNotNull(result);
        }
    }
}
