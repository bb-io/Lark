using Blackbird.Applications.Sdk.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Request
{
    public class GetRangeCellsValuesRequest
    {
        [Display("Range")]
        public string Range { get; set; }
    }
}
