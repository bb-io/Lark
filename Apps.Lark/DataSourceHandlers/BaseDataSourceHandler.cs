using Apps.Appname.Api;
using Apps.Appname;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.DataSourceHandlers
{
    public class BaseDataSourceHandler(InvocationContext invocationContext) : Invocable(invocationContext), IAsyncDataSourceHandler
    {
        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken cancellationToken)
        {
            var larkClient = new LarkClient(InvocationContext.AuthenticationCredentialsProviders);

            var sheets = await GetBasesRecursively(larkClient, null, cancellationToken);

            if (!string.IsNullOrEmpty(context.SearchString))
            {
                sheets = sheets.Where(s => s.Name.Contains(context.SearchString)).ToList();
            }

            return sheets.ToDictionary(s => s.Token, s => s.Name);
        }

        private async Task<List<Sheet>> GetBasesRecursively(LarkClient larkClient, string folderToken, CancellationToken cancellationToken)
        {
            var sheets = new List<Sheet>();

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
                    if (file.Type == "bitable")
                    {
                        sheets.Add(new Sheet
                        {
                            Name = file.Name,
                            Token = file.Token
                        });
                    }
                    else if (file.Type == "folder")
                    {
                        var subSheets = await GetBasesRecursively(larkClient, file.Token, cancellationToken);
                        sheets.AddRange(subSheets);
                    }
                }
            }

            return sheets;
        }
    }
}
