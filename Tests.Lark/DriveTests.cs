using Apps.Lark.Actions;
using Tests.Lark.Base;

namespace Tests.Lark
{
    [TestClass]
    public class DriveTests : TestBase
    {
        [TestMethod]
        public async Task CreateFolder_IsSuccess()
        {
            var action = new DriveActions(InvocationContext, FileManager);

            var folderName = new Apps.Lark.Models.Request.CreateFolderRequest { Name = "TestFolder_" + Guid.NewGuid().ToString() };
            var response = await action.CreateFolder(folderName);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task DownloadFile_IsSuccess()
        {
            var action = new DriveActions(InvocationContext, FileManager);

            var folderName = new Apps.Lark.Models.Request.DownloadFileRequest {FileToken= "QWuyb5x1toKAatxWRAEjbiYupIf" };
            var response = await action.DownloadFile(folderName);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task UploadFile_IsSuccess()
        {
            var action = new DriveActions(InvocationContext, FileManager);

            var folderName = new Apps.Lark.Models.Request.UploadFileRequest {/* FolderId = "R9xlfmA9fldU3ddHcSajh8lkpqb",*/ File = new Blackbird.Applications.Sdk.Common.Files.FileReference { Name = "Testing_upload.txt" } };
            var response = await action.UploadFile(folderName);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }
    }
}
