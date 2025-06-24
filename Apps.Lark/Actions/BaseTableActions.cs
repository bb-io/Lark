using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Utilities;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Lark.Actions
{
    [ActionList]
    public class BaseTableActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : Invocable(invocationContext)
    {

        [Action("Search base tables", Description = "Searches base tables")]
        public async Task<BaseTablesResponse> SearchBaseTables([ActionParameter] BaseRequest input)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{input.AppId}/tables", Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<BaseTablesResponseDto>(request);

            return new BaseTablesResponse
            {
                Tables = response.Data?.Items ?? new List<TableItemDto>()
            };
        }

        [Action("Insert row to base table", Description = "Inserts row to base table ")]
        public async Task InsertBaseTableRow([ActionParameter] BaseRequest input,
            [ActionParameter] BaseTableRequest table)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{input.AppId}/tables/{table.TableId}/records", Method.Post);
            var fields = new Dictionary<string, object>
            {
            };
            var body = new { fields };
            request.AddJsonBody(body);
            await larkClient.ExecuteWithErrorHandling<UpdateRecordResponseDto>(request);
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

        [Action("Get base table used range", Description = "Get all non-empty records from base table")]
        public async Task<RecordsResponse> GetBaseRecords(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records?user_id_type=user_id",Method.Get);

            var response = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(request);

            var items = response.Data?.Items ?? new List<RecordItemDto>();

            for (int i = 0; i < items.Count; i++)
            {
                items[i].RowIndex = i;
            }

            var nonEmpty = items
                .Where(item => item.Fields?.Values.Any(v => v != null) ?? false)
                .ToList();

            return new RecordsResponse
            {
                Records = nonEmpty,
                RecordsCount = nonEmpty.Count
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

            var listReq = new RestRequest(
                $"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records?user_id_type=user_id",
                Method.Get);
            var listResp = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(listReq);
            var items = listResp.Data?.Items ?? new List<RecordItemDto>();
            for (int i = 0; i < items.Count; i++)
                items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException($"RowIndex must be between 0 and {items.Count - 1}");

            var selected = items[record.RowIndex];

            if (selected.Fields == null || !selected.Fields.ContainsKey(update.FieldName))
                throw new PluginMisconfigurationException($"Field '{update.FieldName}' does not exist in the record.");

            object valueToUpdate;

            if (update.Attachment != null)
            {
                await using var fs = await fileManagementClient.DownloadAsync(update.Attachment);
                using var ms = new MemoryStream();
                await fs.CopyToAsync(ms);
                var bytes = ms.ToArray();
                var fileName = update.Attachment.Name;
                var size = bytes.Length.ToString();

                var uploadReq = new RestRequest("/drive/v1/medias/upload_all", Method.Post)
                {
                    AlwaysMultipartFormData = true
                };
                uploadReq.AddParameter("file_name", fileName);
                uploadReq.AddParameter("parent_type", "bitable_file");
                uploadReq.AddParameter("parent_node", baseId.AppId);
                uploadReq.AddParameter("size", size);
                uploadReq.AddFile("file", bytes, fileName, "application/octet-stream");

                var uploadResp = await larkClient.ExecuteWithErrorHandling<UpdateAttachmentResponse>(uploadReq);
                var fileToken = uploadResp.Data.FileToken;

                valueToUpdate = new[]
                {
                    new { file_token = fileToken }
                };
            }
            else if (update.NewDateValue.HasValue)
            {
                var dto = new DateTimeOffset(update.NewDateValue.Value.ToUniversalTime());
                valueToUpdate = dto.ToUnixTimeMilliseconds();
            }
            else if (update.NewValues != null && update.NewValues.Any())
            {
                valueToUpdate = update.NewValues;
            }
            else if (!string.IsNullOrEmpty(update.NewValue))
            {
                valueToUpdate = update.NewValue;
            }
            else
            {
                throw new PluginMisconfigurationException("No new value provided. Specify NewValue, NewValues, NewDateValue or Attachment.");
            }

            var fieldsDict = new Dictionary<string, object>
            {
                [update.FieldName] = valueToUpdate
            };
            var body = new { fields = fieldsDict };

            var updateReq = new RestRequest(
                $"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{selected.RecordId}",
                Method.Put);
            updateReq.AddJsonBody(body);

            var updateResp = await larkClient.ExecuteWithErrorHandling<UpdateRecordResponseDto>(updateReq);

            var updatedFields = selected.Fields.ToDictionary(kv => kv.Key, kv => kv.Value as object);
            if (updateResp.Data?.Record?.Fields != null)
            {
                foreach (var f in updateResp.Data.Record.Fields)
                    updatedFields[f.Key] = f.Value;
            }

            return new UpdateRecordDataDto
            {
                Record = new RecordItemDto
                {
                    Fields = updatedFields,
                    Id = selected.Id,
                    RecordId = selected.RecordId,
                    RowIndex = selected.RowIndex
                }
            };
        }

        [Action("Get person entry from base table record", Description = "Gets person entry from base table record")]
        public async Task<PersonFieldResponse> GetPersonEntry([ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var request = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records?user_id_type=user_id", Method.Get);
            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(request);
            var items = recordsResponse.Data?.Items ?? new List<RecordItemDto>();

            for (int i = 0; i < items.Count; i++)
                items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException($"Row index must be from 0 to {items.Count - 1}");

            var selectedRecord = items[record.RowIndex];

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

        [Action("Get date entries from base table record", Description = "Gets date entries from base table record as DateTime")]
        public async Task<DateFieldResponse> GetDateEntries(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var requestRecords = new RestRequest(
                $"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records?user_id_type=user_id",
                Method.Get);
            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(requestRecords);
            var items = recordsResponse.Data?.Items ?? new List<RecordItemDto>();
            for (int i = 0; i < items.Count; i++) items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException($"Row index must be from 0 to {items.Count - 1}");

            var selectedRecord = items[record.RowIndex];

            var allFields = new List<FieldItem>();
            string? pageToken = null;
            do
            {
                var requestFields = new RestRequest(
                    $"/bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields",
                    Method.Get);
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

            var dateFields = allFields.Where(f => f.Type == 5).ToList();
            var dateEntries = new List<DateFieldEntry>();

            foreach (var df in dateFields)
            {
                if (selectedRecord.Fields != null
                    && selectedRecord.Fields.TryGetValue(df.FieldName, out var rawValue)
                    && rawValue != null)
                {
                    if (long.TryParse(rawValue.ToString(), out var ms))
                    {
                        var dto = DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
                        dateEntries.Add(new DateFieldEntry
                        {
                            FieldId = df.FieldId,
                            FieldName = df.FieldName,
                            Date = dto
                        });
                    }
                }
            }

            return new DateFieldResponse
            {
                DateFields = dateEntries
            };
        }



        [Action("Download attachments from base table record", Description = "Downloads all attachments from a record")]
        public async Task<DownloadAttachmentsResponse> DownloadAttachments(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var recReq = new RestRequest(
                $"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records?user_id_type=user_id",
                Method.Get);
            var recResp = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(recReq);
            var items = recResp.Data?.Items ?? new List<RecordItemDto>();
            for (int i = 0; i < items.Count; i++) items[i].RowIndex = i;

            if (record.RowIndex < 0 || record.RowIndex >= items.Count)
                throw new PluginMisconfigurationException($"Row index must be from 0 to {items.Count - 1}");

            var selected = items[record.RowIndex];

            var allFields = new List<FieldItem>();
            string? pageToken = null;
            do
            {
                var fldReq = new RestRequest(
                    $"/bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields",
                    Method.Get);
                if (!string.IsNullOrEmpty(pageToken))
                    fldReq.AddParameter("page_token", pageToken);

                var fldResp = await larkClient.ExecuteWithErrorHandling<FieldsResponseDto>(fldReq);
                if (fldResp?.Data?.Items != null)
                {
                    allFields.AddRange(fldResp.Data.Items);
                    pageToken = fldResp.Data.HasMore ? fldResp.Data.PageToken : null;
                }
                else pageToken = null;
            } while (!string.IsNullOrEmpty(pageToken));

            var attFields = allFields.Where(f => f.Type == 17).ToList();

            var result = new DownloadAttachmentsResponse();

            foreach (var fld in attFields)
            {
                if (selected.Fields == null
                 || !selected.Fields.TryGetValue(fld.FieldName, out var raw)
                 || raw == null) continue;

                var tokens = (raw is JArray arr)
                    ? arr.ToObject<List<JObject>>()
                          .Select(o => o.Value<string>("file_token"))
                          .Where(t => !string.IsNullOrEmpty(t))
                          .ToList()
                    : new List<string>();

                if (!tokens.Any()) continue;

                var batchReq = new RestRequest("/drive/v1/medias/batch_get_tmp_download_url", Method.Get);
                foreach (var t in tokens)
                    batchReq.AddParameter("file_tokens", t);

                var batchResp = await larkClient.ExecuteWithErrorHandling<BatchUrlResponseDto>(batchReq);

                foreach (var item in batchResp.Data.TmpDownloadUrls)
                {
                    var fileContent = await FileDownloader.DownloadFileBytes(item.TmpDownloadUrl);

                    var fileRef = await fileManagementClient
                        .UploadAsync(fileContent.FileStream, fileContent.ContentType, fileContent.Name);

                    result.Files.Add(new FileResponse { File = fileRef });
                }
            }
            return result;
        }
    }
}
