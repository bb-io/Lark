using Apps.Appname;
using Apps.Appname.Api;
using Apps.Lark.Constants;
using Apps.Lark.Models.Dtos;
using Apps.Lark.Models.Request;
using Apps.Lark.Models.Response;
using Apps.Lark.Polling.Models;
using Apps.Lark.Utils;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Models;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Apps.Lark.Actions
{
    [ActionList("Bases")]
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
        public async Task<RecordResponse> GetRecord(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{record.RecordID}", Method.Get);
            var recordsResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);
            var recordsResponseContent = recordsResponse.Content
                ?? throw new PluginMisconfigurationException("Empty content received for the record.");

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(item => item.FieldName, item => item);

            var receivedRecord = BaseRecordJsonParser.ConvertToRecord(recordsResponseContent, schemaByFieldName)
                ?? throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");

            return new RecordResponse
            {
                BaseId = baseId.AppId,
                TableId = table.TableId,
                RecordId = receivedRecord.RecordId,
                Fields = receivedRecord.Fields,
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

            var selected = items.FirstOrDefault(r => r.RecordId == record.RecordID);
            if (selected is null)
                throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");

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
                    RecordId = selected.RecordId
                }
            };
        }

        [Action("Get person entry from base table record", Description = "Gets person entry from base table record")]
        public async Task<PersonFieldResponse> GetPersonEntry([ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record,
            [ActionParameter] GetPersonFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var fieldSchema = await BaseFiledSchemaParser.GetFieldSchema(larkClient, baseId.AppId, table.TableId, field.FieldId);

            if (fieldSchema.FieldTypeId != 11 && fieldSchema.FieldTypeId != 1003 && fieldSchema.FieldTypeId != 1004)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (Type: {fieldSchema.FieldTypeName}) is not a person field");

            if (fieldSchema.FieldName.Contains("'"))
                throw new PluginMisconfigurationException($"Fields with a single quote in name are not supported.");

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{record.RecordID}?user_id_type=user_id", Method.Get);
            var recordResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);

            var recordResponseJToken = JToken.Parse(recordResponse.Content ?? "")
                ?? throw new PluginMisconfigurationException("No response content received from record retrieval.");

            var fieldJToken = recordResponseJToken.SelectToken($"$.data.record.fields['{fieldSchema.FieldName}']");

            if (fieldJToken == null)
                return new PersonFieldResponse
                {
                    Person = new PersonData
                    {
                        Id = "",
                        Name = ""
                    }
                };

            var users = new List<JObject>();
            if (fieldJToken.Type == JTokenType.Array)
            {
                users = fieldJToken.Cast<JObject>().ToList();
            }
            else if (fieldJToken.Type == JTokenType.Object)
            {
                users.Add(fieldJToken.ToObject<JObject>());
            }

            if (!users.Any())
                return new PersonFieldResponse
                {
                    Person = new PersonData
                    {
                        Id = "",
                        Name = ""
                    }
                };

            var user = users.First();
            var userId = user["id"]?.ToString();
            var userName = user["name"]?.ToString();


            return new PersonFieldResponse
            {
                Person = new PersonData
                {
                    Id = userId ?? "",
                    Name = userName ?? ""
                }
            };
        }

        [Action("Get date entry from base table record", Description = "Gets date entry from base table record as DateTime")]
        public async Task<DateFieldResponse> GetDateEntries(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record,
            [ActionParameter] GetDateFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var fieldSchema = await BaseFiledSchemaParser.GetFieldSchema(larkClient, baseId.AppId, table.TableId, field.FieldId);

            if (fieldSchema.FieldTypeId != 5 && fieldSchema.FieldTypeId != 1001 && fieldSchema.FieldTypeId != 1002)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}', Type: {fieldSchema.FieldTypeName}) is not a date field");

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{record.RecordID}", Method.Get);
            var recordResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);
            var recordResponseJToken = JToken.Parse(recordResponse.Content ?? "")
                ?? throw new PluginMisconfigurationException("No response content received from record retrieval.");

            var fieldJToken = recordResponseJToken.SelectToken($"$.data.record.fields['{fieldSchema.FieldName}']");

            if (fieldJToken == null)
                return new DateFieldResponse
                {
                    DateValue = null
                };

            var dateString = fieldJToken.Value<string>();
            if (!long.TryParse(dateString, out var unixTimestampMs))
                return new DateFieldResponse
                {
                    DateValue = null
                };

            var dateValue = DateTimeOffset.FromUnixTimeMilliseconds(unixTimestampMs).UtcDateTime;

            return new DateFieldResponse
            {
                DateValue = dateValue
            };
        }


        [Action("Get text entry from base table record", Description = "Gets a text entry from a base table record")]
        public async Task<TextFieldResponse> GetTextEntry(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record,
            [ActionParameter] GetTextFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var fieldSchema = await BaseFiledSchemaParser.GetFieldSchema(larkClient, baseId.AppId, table.TableId, field.FieldId);

            if (fieldSchema.FieldTypeId != 1 && fieldSchema.FieldTypeId != 3 && fieldSchema.FieldTypeId != 15)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (Type: {fieldSchema.FieldTypeName}) is not a text field");

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{record.RecordID}", Method.Get);
            var recordResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);
            var recordResponseJToken = JToken.Parse(recordResponse.Content ?? "")
                ?? throw new PluginMisconfigurationException("No response content received from record retrieval.");

            string textValue;
            if (fieldSchema.FieldTypeId == 15)
            {
                var fieldJToken = recordResponseJToken.SelectToken($"$.data.record.fields['{fieldSchema.FieldName}'].link")
                    ?? throw new PluginMisconfigurationException($"Link field '{fieldSchema.FieldName}' ('{fieldSchema.FieldTypeName}') not found or empty in record '{record.RecordID}'.");

                textValue = fieldJToken.Value<string>()
                    ?? throw new PluginMisconfigurationException($"Failed to retrieve link value for field '{fieldSchema.FieldName}' in record '{record.RecordID}'.");
            }
            else
            {
                var fieldJToken = recordResponseJToken.SelectToken($"$.data.record.fields['{fieldSchema.FieldName}']")
                    ?? throw new PluginMisconfigurationException($"Text field '{fieldSchema.FieldName}' ('{fieldSchema.FieldTypeName}') not found or empty in record '{record.RecordID}'.");

                textValue = fieldJToken.Value<string>() ?? string.Empty;
            }

            return new TextFieldResponse
            {
                TextValue = textValue
            };
        }

        [Action("Get multiple option value from base table record", Description = "Gets a multiple select value from a base table record")]
        public async Task<MultiOptionResponse> GetMultiOptionValueFromRecord(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record,
            [ActionParameter] GetFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var fieldSchema = tableSchemaResponse.Data.Items
                .Where(x => x.FieldTypeId == BaseFieldTypes.MultipleOptions && x.FieldId == field.FieldId)
                .FirstOrDefault();

            if (fieldSchema == null)
                throw new PluginMisconfigurationException($"Field is not found in table or is not a multiple option field");

            if (fieldSchema.FieldTypeName.Contains("'"))
                throw new PluginMisconfigurationException($"Fields with a single quote in name are not supported.");

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{record.RecordID}", Method.Get);
            var recordResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);
            var recordResponseJToken = JToken.Parse(recordResponse.Content ?? "")
                ?? throw new PluginMisconfigurationException("No response content received from record retrieval.");

            List<string> selectedValues = [];

            var fieldJToken = recordResponseJToken.SelectToken($"$.data.record.fields['{fieldSchema.FieldName}']");
            if (fieldJToken?.Type == JTokenType.Array)
            {
                selectedValues = fieldJToken
                    .Select(v => v?.Value<string>() ?? string.Empty)
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();
            }

            return new MultiOptionResponse
            {
                SelectedValues = selectedValues
            };
        }

        [Action("Get number entry from base table record", Description = "Gets a number entry from a base table record")]
        public async Task<NumberFieldResponse> GetNumberEntry(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record,
            [ActionParameter] GetNumberFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var fieldSchema = await BaseFiledSchemaParser.GetFieldSchema(larkClient, baseId.AppId, table.TableId, field.FieldId);

            if (fieldSchema.FieldTypeId != 2)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (Type: {fieldSchema.FieldTypeName}) is not a number field");

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{record.RecordID}", Method.Get);
            var recordResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);
            var recordResponseJToken = JToken.Parse(recordResponse.Content ?? "")
                ?? throw new PluginMisconfigurationException("No response content received from record retrieval.");

            var fieldJToken = recordResponseJToken.SelectToken($"$.data.record.fields['{fieldSchema.FieldName}']");

            if (fieldJToken == null)
                return new NumberFieldResponse
                {
                    NumberValue = null
                };

            var numberString = fieldJToken.Value<string>();
            if (!double.TryParse(numberString, out var numberValue))
                return new NumberFieldResponse
                {
                    NumberValue = null
                };

            return new NumberFieldResponse
            {
                NumberValue = numberValue
            };
        }      


        [Action("Download attachments from base table record", Description = "Downloads all attachments from a record")]
        public async Task<DownloadAttachmentsResponse> DownloadAttachments(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record,
            [ActionParameter] GetDownloadFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);
            var result = new DownloadAttachmentsResponse();

            var fieldSchema = await BaseFiledSchemaParser.GetFieldSchema(larkClient, baseId.AppId, table.TableId, field.FieldId);

            if (fieldSchema.FieldTypeId != 17)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (Type: {fieldSchema.FieldTypeName}) is not an attachment field (expected type: 17).");

            if (fieldSchema.FieldName.Contains("'"))
                throw new PluginMisconfigurationException($"Fields with a single quote in name are not supported.");

            var recordRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/{record.RecordID}", Method.Get);
            var recordResponse = await larkClient.ExecuteWithErrorHandling(recordRequest);
            var recordResponseJToken = JToken.Parse(recordResponse.Content ?? "")
                ?? throw new PluginMisconfigurationException("No response content received from record retrieval.");

            var fieldJToken = recordResponseJToken.SelectToken($"$.data.record.fields['{fieldSchema.FieldName}']");

            if (fieldJToken == null)
                return result;

            var attachments = new List<(string Url, string Name, string ContentType)>();
            if (fieldJToken.Type == JTokenType.Array)
            {
                attachments = fieldJToken.ToObject<List<JObject>>()
                    .Select(o => (
                        Url: o.Value<string>("url"),
                        Name: o.Value<string>("name"),
                        ContentType: o.Value<string>("type")))
                    .Where(a => !string.IsNullOrEmpty(a.Url) && !string.IsNullOrEmpty(a.Name))
                    .ToList();
            }
            else if (fieldJToken.Type == JTokenType.Object)
            {
                var obj = fieldJToken.ToObject<JObject>();
                var url = obj.Value<string>("url");
                var name = obj.Value<string>("name");
                var contentType = obj.Value<string>("type");
                if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(name))
                    attachments.Add((url, name, contentType));
            }

            if (!attachments.Any())
                return result;

            foreach (var attachment in attachments)
            {
                var downloadRequest = new RestRequest(attachment.Url, Method.Get);
                var downloadResponse = await larkClient.ExecuteAsync(downloadRequest);
                if (!downloadResponse.IsSuccessStatusCode)
                    continue;

                string contentType = attachment.ContentType ?? downloadResponse.ContentType ?? "application/octet-stream";
                var fileContent = new BlackbirdFile(new MemoryStream(downloadResponse.RawBytes), attachment.Name, contentType);
                var fileRef = await fileManagementClient.UploadAsync(fileContent.FileStream, contentType, attachment.Name);
                result.Files.Add(new FileResponse { File = fileRef });
            }
            return result;
        }
    }
}
