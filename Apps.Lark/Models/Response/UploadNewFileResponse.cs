using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Response
{
    public class UploadNewFileResponse
    {
        [Display("File")]
        public string FileToken { get; set; } = default!;

        [Display("File name")]
        public string Name { get; set; } = default!;

        [Display("Parent folder")]
        public string? ParentFolderToken { get; set; }

        [Display("File URL")]
        public string? Url { get; set; }
    }
}
