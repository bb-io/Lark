namespace Apps.Lark.Models.Response
{
    public class DriveFilesResponse
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public DriveFilesData Data { get; set; }
    }

    public class DriveFilesData
    {
        public List<DriveFile> Files { get; set; }
        public bool HasMore { get; set; }
    }

    public class DriveFile
    {
        public string Name { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
    }

    public class Folder
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }

    public class Sheet
    {
        public string Name { get; set; }
        public string Token { get; set; }
    }
}
