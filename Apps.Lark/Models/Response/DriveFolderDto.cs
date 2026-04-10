using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class DriveFolderDto
    {
        [Display("Folder")]
        public string FolderToken { get; set; }

        [Display("Folder name")]
        public string Name { get; set; }

        [Display("Parent folder")]
        public string? ParentFolderToken { get; set; }

        [Display("Folder URL")]
        public string? Url { get; set; }
    }
}
