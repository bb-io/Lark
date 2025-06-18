using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using Blackbird.Applications.Sdk.Common.Exceptions;

namespace Apps.Lark.Actions
{
    [ActionList]
    public class BaseTableActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
    {
        //[Action("Create base table", Description = "Creates base table")]
        //public async Task<> CreateBaseTable([ActionParameter] CreateSpreadsheetRequest input)
        //{
        //    var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

        //    var request = new RestRequest("/sheets/v3/spreadsheets", Method.Post);


        //}

        [Action("Search base tables", Description = "Searches base tables")]
        public async Task<BaseTablesResponse> SearchBaseTables([ActionParameter] BaseRequest input)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{input.AppId}/tables", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<BaseTablesResponseDto>(request);

            return new BaseTablesResponse
            {
               Tables=response.Data?.Items ?? new List<TableItemDto>()
            };
        }

        [Action("Get base record", Description = "Gets record from base table")]
        public async Task<RecordResponse> GetRecord([ActionParameter] BaseRequest baseId, [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(request);
            var items = response.Data?.Items ?? new List<RecordItemDto>();

            for (int i = 0; i < items.Count; i++)
                items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException(
                    $"Row index must be from 0 to {items.Count - 1}");

            var selected = items[record.RowIndex];

            return new RecordResponse
            {
                Values = selected
            };
        }

        [Action("Update base record", Description = "Updates base record")]
        public async Task<BaseTablesResponse> UpdateRecord([ActionParameter] BaseRequest input,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] UpdateRecordRequest update,
            [ActionParameter] GetBaseRecord record
            )
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);


            var request = new RestRequest($"bitable/v1/apps/{input.AppId}/tables", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<BaseTablesResponseDto>(request);

            return new BaseTablesResponse
            {
                Tables = response.Data?.Items ?? new List<TableItemDto>()
            };
        }
    }
}
