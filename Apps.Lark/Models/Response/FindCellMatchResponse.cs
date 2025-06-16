using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class FindCellMatchResponse
    {
        [Display("Matched cells")]
        public List<string>? MatchedCells { get; set; }

        [Display("Matched formula cells")]
        public List<string>? MatchedFormulaCells { get; set; }
    }
}
