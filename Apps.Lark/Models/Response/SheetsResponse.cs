using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Response
{
    public class SheetsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public SpreadsheetSheetsData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
    public class SpreadsheetSheetsData
    {
        [JsonProperty("sheets")]
        public List<SheetInfo> Sheets { get; set; }
    }

    public class SheetInfo
    {
        [JsonProperty("sheet_id")]
        public string SheetId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }
}
