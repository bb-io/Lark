using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Lark.Models.Response
{
    public class CreateSpreadsheetResult
    {
        [Display("Spreadsheet ID")]
        public string SpreadsheetId { get; set; }

        [Display("Folder ID")]
        public string FolderId { get; set; }

        [Display("Title")]
        public string Title { get; set; }

        [Display("URL")]
        public string Url { get; set; }
    }

    public class CreateSpreadsheetResponse
    {
        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("data")]
        public SpreadsheetData Data { get; set; }

        [JsonProperty("msg")]
        [Display("Message")]
        public string Msg { get; set; }
    }
    public class SpreadsheetData
    {
        [JsonProperty("spreadsheet")]
        public Spreadsheet Spreadsheet { get; set; }
    }

    public class Spreadsheet
    {
        [JsonProperty("folder_token")]
        [Display("Folder ID")]
        public string FolderToken { get; set; }

        [JsonProperty("spreadsheet_token")]
        [Display("Spreadsheet ID")]
        public string SpreadsheetToken { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
