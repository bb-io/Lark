using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Security.AccessControl;

namespace Apps.Lark.Actions
{
    [ActionList("Drive")]
    public class DriveActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
    {
        [Action("Create folder")]
        public async Task<DriveFolderDto> CreateFolder([ActionParameter] CreateFolderRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.Name))
                throw new PluginMisconfigurationException("Folder name is required.");

            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);

            var parentToken = string.IsNullOrWhiteSpace(input.FolderId)
                ? await GetRootFolderToken(larkClient)
                : input.FolderId!;

            var req = new RestRequest("/drive/v1/files/create_folder", Method.Post)
                .AddJsonBody(new
                {
                    name = input.Name,
                    folder_token = parentToken
                });

            var resp = await larkClient.ExecuteWithErrorHandling<JObject>(req);
            var data = resp["data"] ?? throw new PluginApplicationException("Lark Drive response has no 'data' node.");

            var folderToken = (string?)data["token"]
                              ?? (string?)data["folder_token"]
                              ?? throw new PluginApplicationException("Create folder response does not contain a folder token.");

            return new DriveFolderDto
            {
                FolderToken = folderToken,
                Name = (string?)data["name"] ?? input.Name,
                ParentFolderToken = (string?)data["parent_token"] ?? parentToken,
                Url = (string?)data["url"]
            };
        }

        [Action("Download file", Description = "Download a binary file from Lark Drive by file token")]
        public async Task<FileResponse> DownloadFile([ActionParameter] DownloadFileRequest input)
        {
            if (string.IsNullOrWhiteSpace(input.FileToken))
                throw new PluginMisconfigurationException("File token is required.");

            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);

            var req = new RestRequest($"/drive/v1/files/{input.FileToken}/download", Method.Get);

            var resp = await larkClient.ExecuteAsync(req);
            if (!resp.IsSuccessful || resp.RawBytes is null || resp.RawBytes.Length == 0)
                throw new PluginApplicationException($"Failed to download file. HTTP {(int)resp.StatusCode}. Content: {resp.Content}");

            string? headerName = resp.Headers?
                .FirstOrDefault(h => string.Equals(h.Name, "Content-Disposition", StringComparison.OrdinalIgnoreCase))
                ?.Value?.ToString();

            string? nameFromHeader = null;
            if (!string.IsNullOrEmpty(headerName))
            {
                const string marker = "filename=";
                var idx = headerName.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    nameFromHeader = headerName[(idx + marker.Length)..].Trim(' ', '"', '\'');
            }

            var finalBase = !string.IsNullOrWhiteSpace(input.FileName)
                ? input.FileName!.Trim()
                : (nameFromHeader ?? input.FileToken);

            var finalExt = !string.IsNullOrWhiteSpace(input.Extension)
                ? input.Extension!.Trim().TrimStart('.')
                : null;

            var finalName = finalExt is null ? finalBase : $"{finalBase}.{finalExt}";

            var contentType = !string.IsNullOrWhiteSpace(resp.ContentType)
                ? resp.ContentType!
                : "application/octet-stream";

            using var ms = new MemoryStream(resp.RawBytes);
            var fileRef = await fileManagementClient.UploadAsync(ms, contentType, finalName);

            return new FileResponse { File = fileRef };
        }

        [Action("Upload file", Description = "Upload a binary file to Lark Drive")]
        public async Task<UploadNewFileResponse> UploadFile([ActionParameter] UploadFileRequest input)
        {
            if (input.File is null)
                throw new PluginMisconfigurationException("File is required.");

            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);

            var targetFolder = string.IsNullOrWhiteSpace(input.FolderId)
                ? await GetRootFolderToken(larkClient)
                : input.FolderId!;

            await using var src = await fileManagementClient.DownloadAsync(input.File);
            using var ms = new MemoryStream();
            await src.CopyToAsync(ms);
            if (ms.Length == 0)
                throw new PluginMisconfigurationException("Input file is empty.");
            var bytes = ms.ToArray();
            var contentType = input.File.ContentType ?? "application/octet-stream";

            var fileName = string.IsNullOrWhiteSpace(input.FileName)
                ? (string.IsNullOrWhiteSpace(input.File.Name) ? "upload.bin" : input.File.Name)
                : input.FileName!.Trim();

            var checkReq = new RestRequest("/drive/v1/files", Method.Get)
                .AddQueryParameter("folder_token", targetFolder);
            var checkResp = await larkClient.ExecuteAsync(checkReq);
            if (!checkResp.IsSuccessful)
                throw new PluginApplicationException($"Parent token validation failed. HTTP {(int)checkResp.StatusCode}. Content: {checkResp.Content}");

            async Task<(bool ok, JObject json, RestResponse raw)> TryUpload(Dictionary<string, string> fields)
            {
                var req = new RestRequest("/drive/v1/files/upload_all", Method.Post)
                {
                    AlwaysMultipartFormData = true
                };

                req.AddParameter("file_name", fileName);
                foreach (var kv in fields)
                    req.AddParameter(kv.Key, kv.Value);

                if (!fields.ContainsKey("file_size") && !fields.ContainsKey("size"))
                    req.AddParameter("file_size", bytes.LongLength.ToString());

                req.AddFile("file", bytes, fileName, contentType);

                var raw = await larkClient.ExecuteAsync(req);
                if (!raw.IsSuccessful || string.IsNullOrWhiteSpace(raw.Content))
                    return (false, new JObject(), raw);

                var j = JObject.Parse(raw.Content);
                return (((int?)j["code"] ?? -1) == 0, j, raw);
            }

            var (ok1, j1, raw1) = await TryUpload(new Dictionary<string, string>
            {
                ["folder_token"] = targetFolder
            });
            if (ok1) return MapUploadResponse(j1, targetFolder, fileName);

            var (ok2, j2, raw2) = await TryUpload(new Dictionary<string, string>
            {
                ["parent_token"] = targetFolder
            });
            if (ok2) return MapUploadResponse(j2, targetFolder, fileName);

            var (ok3, j3, raw3) = await TryUpload(new Dictionary<string, string>
            {
                ["parent_type"] = "explorer",
                ["parent_node"] = targetFolder,
                ["size"] = bytes.LongLength.ToString()
            });
            if (ok3) return MapUploadResponse(j3, targetFolder, fileName);

            var last = j3.HasValues ? j3 : (j2.HasValues ? j2 : j1);
            var raw = string.IsNullOrWhiteSpace(raw3.Content) ? (string.IsNullOrWhiteSpace(raw2.Content) ? raw1.Content : raw2.Content) : raw3.Content;
            var logId = last["error"]?["log_id"] ?? last["log_id"];
            throw new PluginApplicationException(
                $"Upload failed. Code {(int?)last["code"]}: {last["msg"]}. " +
                $"{(logId != null ? $"LogId: {logId}. " : string.Empty)}Content: {raw}");
        }

        private static UploadNewFileResponse MapUploadResponse(JObject j, string parentToken, string fallbackName)
        {
            var data = j["data"] ?? throw new PluginApplicationException("Lark Drive response has no 'data' node.");
            var token = (string?)data["token"] ?? (string?)data["file_token"]
                        ?? throw new PluginApplicationException("Upload response does not contain a file token.");
            return new UploadNewFileResponse
            {
                FileToken = token,
                Name = (string?)data["name"] ?? fallbackName,
                ParentFolderToken = (string?)data["parent_token"] ?? parentToken,
                Url = (string?)data["url"]
            };
        }

        private static async Task<string> GetRootFolderToken(LarkClient larkClient)
        {
            var req = new RestRequest("/drive/explorer/v2/root_folder/meta", Method.Get);
            var resp = await larkClient.ExecuteWithErrorHandling<JObject>(req);

            var code = (int?)resp["code"] ?? -1;
            if (code != 0)
                throw new PluginApplicationException($"Lark error {code}: {resp["msg"]?.ToString() ?? "Unknown error"}");

            var data = resp["data"] ?? throw new PluginApplicationException("Response missing 'data' node.");

            var root = (string?)data["token"]
                       ?? (string?)data["root_token"]
                       ?? (string?)data["root_folder_token"];

            if (string.IsNullOrWhiteSpace(root))
                throw new PluginApplicationException("Root folder token is missing in response.");

            return root!;
        }
    }
}
