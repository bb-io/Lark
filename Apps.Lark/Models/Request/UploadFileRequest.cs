using Apps.Appname.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Lark.Models.Request
{
    public class UploadFileRequest
    {
        [Display("Parent folder")]
        [DataSource(typeof(FolderDataSourceHandler))]
        public string? FolderId { get; set; }

        [Display("File")]
        public FileReference File { get; set; } = default!;

        [Display("File name override")]
        public string? FileName { get; set; }
    }
}
