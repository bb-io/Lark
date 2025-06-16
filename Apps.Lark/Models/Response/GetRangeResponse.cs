using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class GetRangeResponse
    {
        [Display("Spreadsheet token")]
        public string? SpreadsheetToken { get; set; }


        [Display("Major dimension")]
        public string MajorDimension { get; set; }

        public string Range { get; set; }

        public int Revision { get; set; }

        public List<List<object>> Values { get; set; }
    }
}
