using Apps.Lark.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Request
{
    public class DownloadFileRequest
    {
        [Display("File")]
        [DataSource(typeof(FileDataSourceHandler))]
        public string FileToken { get; set; } = default!;

        [Display("File name override")]
        public string? FileName { get; set; }

        [Display("Extension override")]
        public string? Extension { get; set; }
    }
}
