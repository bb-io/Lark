using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class FindCellsRequest
    {   
        [Display("Range")]
        public string Range { get; set; }

        [Display("Query to find")]
        public string Query { get; set; }

        [Display("Match case")]
        public bool? MatchCase { get; set; }

        [Display("Match entire cell")]
        public bool? MatchEntireCell { get; set; }

        [Display("Search by regex")]
        public bool? SearchByRegex { get; set; }

        [Display("Include fornulas")]
        public bool? IncludeFormulas { get; set; }
    }
}
