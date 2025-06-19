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
using System.Buffers.Text;
using Newtonsoft.Json.Linq;

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

            var request = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records?user_id_type=user_id", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(request);
            var items = response.Data?.Items ?? new List<RecordItemDto>();

            for (int i = 0; i < items.Count; i++)
                items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException($"Row index must be from 0 to {items.Count - 1}");

            var selected = items[record.RowIndex];

            return new RecordResponse
            {
                Values = selected
            };
        }

        [Action("Update base record", Description = "Updates base record")]
        public async Task<UpdateRecordDataDto> UpdateRecord([ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] UpdateRecordRequest update,
            [ActionParameter] GetBaseRecord record
            )
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records", Method.Get);

            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(request);
            var items = recordsResponse.Data?.Items ?? new List<RecordItemDto>();

            for (int i = 0; i < items.Count; i++)
                items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException($"Row index must be from 0 to {items.Count - 1}");

            var selectedRecord = items[record.RowIndex];

            if (selectedRecord.Fields == null || !selectedRecord.Fields.ContainsKey(update.FieldName))
                throw new PluginMisconfigurationException($"Field '{update.FieldName}' does not exist in the record.");

            var fieldsToUpdate = new Dictionary<string, object> { [update.FieldName] = update.NewValues?.Any() == true ? update.NewValues : update.NewValue };
            var updateBody = new { fields = fieldsToUpdate };

            var updateRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{selectedRecord.RecordId}", Method.Put);
            updateRequest.AddJsonBody(updateBody);

            var updateResponse = await larkClient.ExecuteWithErrorHandling<UpdateRecordResponseDto>(updateRequest);

            var updatedFields = selectedRecord.Fields?.ToDictionary(kv => kv.Key, kv => kv.Value as object) ?? new Dictionary<string, object>();
            if (updateResponse.Data?.Record?.Fields != null)
            {
                foreach (var field in updateResponse.Data.Record.Fields)
                {
                    updatedFields[field.Key] = field.Value;
                }
            }

            var updatedRecord = new RecordItemDto
            {
                Fields = updatedFields,
                Id = selectedRecord.Id,
                RecordId = selectedRecord.RecordId,
                RowIndex = selectedRecord.RowIndex
            };

            return new UpdateRecordDataDto
            {
                Record = updatedRecord
            };
        }

        [Action("Get person entry from base table record", Description = "Gets person entry from base table record")]
        public async Task<PersonFieldResponse> GetPersonEntry([ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records", Method.Get);
            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(request);
            var items = recordsResponse.Data?.Items ?? new List<RecordItemDto>();

            for (int i = 0; i < items.Count; i++)
                items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException($"Row index must be from 0 to {items.Count - 1}");

            var selectedRecord = items[record.RowIndex]; // Record from where we want to gt the value, specified by field type

            var allFields = new List<FieldItem>();
            string? pageToken = null;

            do
            {
                var requestFields = new RestRequest($"/bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
                if (!string.IsNullOrEmpty(pageToken))
                    requestFields.AddParameter("page_token", pageToken);

                var fieldsResponse = await larkClient.ExecuteWithErrorHandling<FieldsResponseDto>(requestFields);
                if (fieldsResponse?.Data?.Items != null)
                {
                    allFields.AddRange(fieldsResponse.Data.Items);
                    pageToken = fieldsResponse.Data.HasMore ? fieldsResponse.Data.PageToken : null;
                }
                else
                {
                    pageToken = null;
                }
            } while (!string.IsNullOrEmpty(pageToken));

            var personFields = allFields.Where(f => f.Type == 11).ToList();

            var personFieldEntries = new List<PersonFieldEntry>();

            foreach (var personField in personFields)
            {
                if (selectedRecord.Fields != null && selectedRecord.Fields.TryGetValue(personField.FieldName, out var fieldValue) && fieldValue != null)
                {
                    var users = new List<PersonData>();

                    if (fieldValue is JArray userArray)
                    {
                        users = userArray.ToObject<List<PersonData>>() ?? new List<PersonData>();
                    }
                    else if (fieldValue is JObject userObject)
                    {
                        var user = userObject.ToObject<PersonData>();
                        if (user != null)
                            users.Add(user);
                    }

                    personFieldEntries.Add(new PersonFieldEntry
                    {
                        FieldName = personField.FieldName,
                        FieldId = personField.FieldId,
                        Users = users
                    });
                }
            }

            return new PersonFieldResponse
            {
                PersonFields = personFieldEntries.Any() ? personFieldEntries : new List<PersonFieldEntry>()
            };



        }
    }
}
