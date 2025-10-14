using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Lark.DataSourceHandlers
{
    public class FileDataSourceHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceHandler
    {
        public async Task<Dictionary<string, string>> GetDataAsync(
        DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);

            var files = await GetFilesRecursively(larkClient, parentFolderToken: null, cancellationToken);

            if (!string.IsNullOrWhiteSpace(context.SearchString))
            {
                var term = context.SearchString!;
                files = files
                    .Where(f => f.Name?.Contains(term, StringComparison.OrdinalIgnoreCase) == true)
                    .ToList();
            }

            return files.ToDictionary(f => f.Token, f => f.Name ?? f.Token);
        }

        private async Task<List<FileItem>> GetFilesRecursively(
            LarkClient larkClient, string? parentFolderToken, CancellationToken cancellationToken)
        {
            var result = new List<FileItem>();

            var req = new RestRequest("/drive/v1/files", Method.Get);
            if (!string.IsNullOrEmpty(parentFolderToken))
                req.AddQueryParameter("folder_token", parentFolderToken);

            var resp = await larkClient.ExecuteWithErrorHandling<DriveFilesResponse>(req);

            if (resp.Code != 0 || resp.Data?.Files == null)
                return result;

            foreach (var f in resp.Data.Files)
            {
                if (!string.Equals(f.Type, "folder", StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(new FileItem
                    {
                        Name = f.Name,
                        Token = f.Token
                    });
                }
                else
                {
                    var children = await GetFilesRecursively(larkClient, f.Token, cancellationToken);
                    result.AddRange(children);
                }
            }

            return result;
        }

        private sealed class FileItem
        {
            public string? Name { get; set; }
            public string Token { get; set; } = default!;
        }
    }
}
