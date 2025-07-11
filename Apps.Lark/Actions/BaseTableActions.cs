﻿using Apps.Appname;
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

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(item => item.FieldName, item => item);

            var schemaJson = JsonConvert.SerializeObject(schemaByFieldName.Values, Formatting.Indented);

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "100");
            searchRecordsRequest.AddJsonBody(new
            {
                filter = new
                {
                    conjunction = "and",
                    conditions = new[]
                    {
                        new
                        {
                            field_name = "Request ID",
                            @operator = "is",
                            value = new[] { record.RecordID }
                        }
                    }
                }
            });

            var recordsResponse = await larkClient.ExecuteWithErrorHandling(searchRecordsRequest);
            var recordsResponseContent = recordsResponse.Content ?? throw new PluginMisconfigurationException("No response content received from records search.");

            var receivedRecords = BaseRecordJsonParser
                .ConvertToRecordsList(recordsResponseContent, schemaByFieldName)
                .ToList();

            if (!receivedRecords.Any())
                throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");

            var selectedRecord = receivedRecords.First();

            return new RecordResponse
            {
                BaseId = baseId.AppId,
                TableId = table.TableId,
                RecordId = selectedRecord.RecordId,
                Fields = selectedRecord.Fields,
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
            [ActionParameter] GetFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(x => x.FieldName, x => x);

            var fieldSchema = tableSchemaResponse.Data.Items.FirstOrDefault(x => x.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Field with ID '{field.FieldId}' not found in table '{table.TableId}'.");
            if (fieldSchema.FieldTypeId != 11 && fieldSchema.FieldTypeId != 1003 && fieldSchema.FieldTypeId != 1004)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (ID: '{field.FieldId}', Type: {fieldSchema.FieldTypeId}) is not a person field");

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search?user_id_type=user_id", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "100");
            searchRecordsRequest.AddJsonBody(new
            {
                filter = new
                {
                    conjunction = "and",
                    conditions = new[]
                    {
                        new
                        {
                            field_name = "Request ID",
                            @operator = "is",
                            value = new[] { record.RecordID }
                        }
                    }
                }
            });

            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(searchRecordsRequest); 
            var recordsResponseContent = recordsResponse.Data
                ?? throw new PluginMisconfigurationException("No response content received from records search.");

            var selectedRecord = recordsResponse.Data?.Items?.FirstOrDefault(r => r.RecordId == record.RecordID)
                ?? throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");
            var rawFields = selectedRecord.Fields
                ?? throw new PluginMisconfigurationException($"No fields found for record '{record.RecordID}'.");

 
            if (!rawFields.TryGetValue(fieldSchema.FieldName, out var rawFieldValue) || rawFieldValue == null)
                throw new PluginMisconfigurationException($"Person field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') not found or empty in record '{record.RecordID}'.");

            var users = new List<JObject>();
            if (rawFieldValue is JArray arr)
            {
                users = arr.Cast<JObject>().ToList();
            }
            else if (rawFieldValue is JObject obj)
            {
                users.Add(obj);
            }

            if (!users.Any())
                throw new PluginMisconfigurationException(
                    $"No valid user data found in person field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') for record '{record.RecordID}'.");

            var user = users.First();
            var userId = user["id"]?.ToString();
            var userName = user["name"]?.ToString();

            if (string.IsNullOrEmpty(userName))
                throw new PluginMisconfigurationException(
                    $"Person field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') in record '{record.RecordID}' has no valid name.");


            if (string.IsNullOrEmpty(userId) )
            {
                userId = userName;
            }

            return new PersonFieldResponse
            {
                Person = new PersonData
                {
                    Id = userId,
                    Name = userName
                }
            };
        }

        [Action("Get date entry from base table record", Description = "Gets date entry from base table record as DateTime")]
        public async Task<DateFieldResponse> GetDateEntries(
            [ActionParameter] BaseRequest baseId,
            [ActionParameter] BaseTableRequest table,
            [ActionParameter] GetBaseRecord record,
            [ActionParameter] GetFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(x => x.FieldName, x => x);

            var fieldSchema = tableSchemaResponse.Data.Items.FirstOrDefault(x => x.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Field with ID '{field.FieldId}' not found in table '{table.TableId}'.");
            if (fieldSchema.FieldTypeId != 5 && fieldSchema.FieldTypeId != 1001 && fieldSchema.FieldTypeId != 1002)
                throw new PluginMisconfigurationException($"Field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') is not a date field.");

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "100");
            searchRecordsRequest.AddJsonBody(new
            {
                filter = new
                {
                    conjunction = "and",
                    conditions = new[]
                    {
                        new
                        {
                            field_name = "Request ID",
                            @operator = "is",
                            value = new[] { record.RecordID }
                        }
                    }
                }
            });

            var recordsResponse = await larkClient.ExecuteWithErrorHandling(searchRecordsRequest);
            var recordsResponseContent = recordsResponse.Content
                ?? throw new PluginMisconfigurationException("No response content received from records search.");

            var receivedRecords = BaseRecordJsonParser
                .ConvertToRecordsList(recordsResponseContent, schemaByFieldName)
                .ToList();

            if (!receivedRecords.Any())
                throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");

            var selectedRecord = receivedRecords.First();

            var dateField = selectedRecord.Fields.FirstOrDefault(f => f.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Date field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') not found in record '{record.RecordID}'.");

            if (!DateTime.TryParse(dateField.FieldValue, out var dateValue))
                throw new PluginMisconfigurationException($"Unable to parse date value '{dateField.FieldValue}' for field '{fieldSchema.FieldName}' in record '{record.RecordID}'.");

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
            [ActionParameter] GetFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(x => x.FieldName, x => x);

            var fieldSchema = tableSchemaResponse.Data.Items.FirstOrDefault(x => x.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Field with ID '{field.FieldId}' not found in table '{table.TableId}'.");
            if (fieldSchema.FieldTypeId != 1 && fieldSchema.FieldTypeId != 3)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (ID: '{field.FieldId}', Type: {fieldSchema.FieldTypeId}) is not a text field");

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "100");
            searchRecordsRequest.AddJsonBody(new
            {
                filter = new
                {
                    conjunction = "and",
                    conditions = new[]
                    {
                        new
                        {
                            field_name = "Request ID",
                            @operator = "is",
                            value = new[] { record.RecordID }
                        }
                    }
                }
            });

            var recordsResponse = await larkClient.ExecuteWithErrorHandling(searchRecordsRequest);
            var recordsResponseContent = recordsResponse.Content
                ?? throw new PluginMisconfigurationException("No response content received from records search.");

            var receivedRecords = BaseRecordJsonParser
                .ConvertToRecordsList(recordsResponseContent, schemaByFieldName)
                .ToList();

            if (!receivedRecords.Any())
                throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");

            var selectedRecord = receivedRecords.First();

            var textField = selectedRecord.Fields.FirstOrDefault(f => f.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Text field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') not found in record '{record.RecordID}'.");

            return new TextFieldResponse
            {
                TextValue = textField.FieldValue ?? string.Empty
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
            [ActionParameter] GetFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(x => x.FieldName, x => x);

            var fieldSchema = tableSchemaResponse.Data.Items.FirstOrDefault(x => x.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Field with ID '{field.FieldId}' not found in table '{table.TableId}'.");
            if (fieldSchema.FieldTypeId != 2)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (ID: '{field.FieldId}', Type: {fieldSchema.FieldTypeId}) is not a number field");

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "100");
            searchRecordsRequest.AddJsonBody(new
            {
                filter = new
                {
                    conjunction = "and",
                    conditions = new[]
                    {
                        new
                        {
                            field_name = "Request ID",
                            @operator = "is",
                            value = new[] { record.RecordID }
                        }
                    }
                }
            });

            var recordsResponse = await larkClient.ExecuteWithErrorHandling(searchRecordsRequest);
            var recordsResponseContent = recordsResponse.Content
                ?? throw new PluginMisconfigurationException("No response content received from records search.");


            var receivedRecords = BaseRecordJsonParser
                .ConvertToRecordsList(recordsResponseContent, schemaByFieldName)
                .ToList();

            if (!receivedRecords.Any())
                throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");

            var selectedRecord = receivedRecords.First();

            var numberField = selectedRecord.Fields.FirstOrDefault(f => f.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Number field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') not found in record '{record.RecordID}'.");

            if (!double.TryParse(numberField.FieldValue, out var numberValue))
                throw new PluginMisconfigurationException(
                    $"Unable to parse number value '{numberField.FieldValue}' for field '{fieldSchema.FieldName}' in record '{record.RecordID}'.");

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
            [ActionParameter] GetFieldRequest field)
        {
            var larkClient = new LarkClient(invocationContext.AuthenticationCredentialsProviders);
            var result = new DownloadAttachmentsResponse();

            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var schemaByFieldName = tableSchemaResponse.Data.Items.ToDictionary(x => x.FieldName, x => x);

            var fieldSchema = tableSchemaResponse.Data.Items.FirstOrDefault(x => x.FieldId == field.FieldId)
                ?? throw new PluginMisconfigurationException($"Field with ID '{field.FieldId}' not found in table '{table.TableId}'.");
            if (fieldSchema.FieldTypeId != 17)
                throw new PluginMisconfigurationException(
                    $"Field '{fieldSchema.FieldName}' (ID: '{field.FieldId}', Type: {fieldSchema.FieldTypeId}) is not an attachment field (expected type: 17).");

            var searchRecordsRequest = new RestRequest($"bitable/v1/apps/{baseId.AppId}/tables/{table.TableId}/records/search", Method.Post);
            searchRecordsRequest.AddQueryParameter("page_size", "1");
            searchRecordsRequest.AddJsonBody(new
            {
                filter = new
                {
                    conjunction = "and",
                    conditions = new[]
                    {
                        new
                        {
                            field_name = "Request ID",
                            @operator = "is",
                            value = new[] { record.RecordID }
                        }
                    }
                }
            });

            var recordsResponse = await larkClient.ExecuteWithErrorHandling<RecordsResponseDto>(searchRecordsRequest);
            var recordsResponseContent = recordsResponse.Data
                ?? throw new PluginMisconfigurationException("No response content received from records search.");

            var selectedRecord = recordsResponse.Data?.Items?.FirstOrDefault(r => r.RecordId == record.RecordID)
                ?? throw new PluginMisconfigurationException($"Record with ID '{record.RecordID}' not found in table {table.TableId}.");

            if (selectedRecord.Fields == null || !selectedRecord.Fields.TryGetValue(fieldSchema.FieldName, out var rawFieldValue) || rawFieldValue == null)
                throw new PluginMisconfigurationException($"Attachment field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') not found or empty in record '{record.RecordID}'.");

            var attachments = new List<(string Url, string Name, string ContentType)>();
            if (rawFieldValue is JArray arr)
            {
                attachments = arr.ToObject<List<JObject>>()
                    .Select(o => (
                        Url: o.Value<string>("url"),
                        Name: o.Value<string>("name"),
                        ContentType: o.Value<string>("type")))
                    .Where(a => !string.IsNullOrEmpty(a.Url) && !string.IsNullOrEmpty(a.Name))
                    .ToList();
            }
            else if (rawFieldValue is JObject obj)
            {
                var url = obj.Value<string>("url");
                var name = obj.Value<string>("name");
                var contentType = obj.Value<string>("type");
                if (!string.IsNullOrEmpty(url) && !string.IsNullOrEmpty(name))
                    attachments.Add((url, name, contentType));
            }

            if (!attachments.Any())
                throw new PluginApplicationException(
                    $"No valid attachments found in field '{fieldSchema.FieldName}' (ID: '{field.FieldId}') for record '{record.RecordID}'.");

            foreach (var attachment in attachments)
            {
                try
                {
                    var downloadRequest = new RestRequest(attachment.Url, Method.Get);
                    var downloadResponse = await larkClient.ExecuteAsync(downloadRequest);
                    if (!downloadResponse.IsSuccessStatusCode)
                        throw new PluginApplicationException($"File download failed; Code: {downloadResponse.StatusCode}; URL: {attachment.Url}");

                    string contentType = attachment.ContentType ?? downloadResponse.ContentType ?? "application/octet-stream";
                    var fileContent = new BlackbirdFile(new MemoryStream(downloadResponse.RawBytes), attachment.Name, contentType);
                    var fileRef = await fileManagementClient.UploadAsync(fileContent.FileStream, contentType, attachment.Name);
                    result.Files.Add(new FileResponse { File = fileRef });
                }
                catch (Exception ex)
                {
                    throw new PluginMisconfigurationException(
                        $"Failed to process attachment '{attachment.Name}' in field '{fieldSchema.FieldName}' for record '{record.RecordID}': {ex.Message}");
                }
            }

            return result;
        }   
    }
}
