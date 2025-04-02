using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Response
{
    public class GetRangeCellsValuesResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public GetRangeCellsValuesData Data { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }
    }
    public class GetRangeCellsValuesData
    {
        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("spreadsheetToken")]
        public string SpreadsheetToken { get; set; }

        [JsonProperty("valueRange")]
        public ValueRange ValueRange { get; set; }
    }

    public class ValueRange
    {
        [JsonProperty("majorDimension")]
        public string MajorDimension { get; set; }

        [JsonProperty("range")]
        public string Range { get; set; }

        [JsonProperty("revision")]
        public int Revision { get; set; }

        [JsonProperty("values")]
        public List<List<object>> Values { get; set; }
    }
}
