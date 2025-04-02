using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Response
{
    public class UpdateRowsResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public UpdateRowsData Data { get; set; }
    }

    public class UpdateRowsData
    {
        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("spreadsheetToken")]
        public string SpreadsheetToken { get; set; }

        [JsonProperty("tableRange")]
        public string TableRange { get; set; }

        [JsonProperty("updates")]
        public UpdateRowsUpdates Updates { get; set; }
    }

    public class UpdateRowsUpdates
    {
        [JsonProperty("spreadsheetToken")]
        public string SpreadsheetToken { get; set; }

        [JsonProperty("updatedRange")]
        public string UpdatedRange { get; set; }

        [JsonProperty("updatedRows")]
        public int UpdatedRows { get; set; }

        [JsonProperty("updatedColumns")]
        public int UpdatedColumns { get; set; }

        [JsonProperty("updatedCells")]
        public int UpdatedCells { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }
    }
}
