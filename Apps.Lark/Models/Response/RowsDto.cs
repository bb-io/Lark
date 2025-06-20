using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class RowsDto
    {
        [Display("Range")]
        public string Range { get; set; }

        public List<RowDto> Rows { get; set; }

        [Display("Rows count")]
        public double RowsCount { get; set; }
    }
    public class RowDto
    {
        [Display("Row ID")]
        public int RowId { get; set; }

        public List<string> Values { get; set; }
    }
}
