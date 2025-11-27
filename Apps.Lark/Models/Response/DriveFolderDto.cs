using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class DriveFolderDto
    {
        [Display("Folder token")]
        public string FolderToken { get; set; }

        [Display("Name")]
        public string Name { get; set; }

        [Display("Parent token")]
        public string? ParentFolderToken { get; set; }

        [Display("Web URL")]
        public string? Url { get; set; }
    }
}
