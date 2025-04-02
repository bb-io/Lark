using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Appname.Handlers;
public class FolderDataSourceHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceHandler
{
    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
    {
        var larkClient = new Apps.Appname.Api.LarkClient(InvocationContext.AuthenticationCredentialsProviders);

        var folders = await GetFoldersRecursively(larkClient, null, cancellationToken);

        if (!string.IsNullOrEmpty(context.SearchString))
        {
            folders = folders.Where(f => f.Name.Contains(context.SearchString)).ToList();
        }

        return folders.ToDictionary(f => f.Name, f => f.Token);
    }

    private async Task<List<Folder>> GetFoldersRecursively(Apps.Appname.Api.LarkClient larkClient, string folderToken, CancellationToken cancellationToken)
    {
        var folders = new List<Folder>();

        var request = new RestRequest("/drive/v1/files", Method.Get);
        if (!string.IsNullOrEmpty(folderToken))
        {
            request.AddQueryParameter("folder_token", folderToken);
        }

        var response = await larkClient.ExecuteWithErrorHandling<DriveFilesResponse>(request);

        if (response.Code == 0 && response.Data?.Files != null)
        {
            foreach (var file in response.Data.Files)
            {
                if (file.Type == "folder")
                {
                    var folder = new Folder
                    {
                        Name = file.Name,
                        Token = file.Token
                    };

                    folders.Add(folder);

                    var subFolders = await GetFoldersRecursively(larkClient, folder.Token, cancellationToken);
                    folders.AddRange(subFolders);
                }
            }
        }

        return folders;
    }
}
