using Apps.Appname.Handlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Lark.Models.Request
{
    public class CreateFolderRequest
    {
        [Display("Parent folder")]
        [DataSource(typeof(FolderDataSourceHandler))]
        public string? FolderId { get; set; }

        [Display("Folder name")]
        public string Name { get; set; }
    }
}
