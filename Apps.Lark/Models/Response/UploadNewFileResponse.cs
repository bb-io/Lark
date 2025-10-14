using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class UploadNewFileResponse
    {
        [Display("File token")]
        public string FileToken { get; set; } = default!;

        [Display("Name")]
        public string Name { get; set; } = default!;

        [Display("Parent token")]
        public string? ParentFolderToken { get; set; }

        [Display("Web URL")]
        public string? Url { get; set; }
    }
}
