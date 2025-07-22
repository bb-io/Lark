using Apps.Appname.Api;
using Apps.Lark.Models.Dtos;
using Blackbird.Applications.Sdk.Common.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Utils
{
    public static class BaseFiledSchemaParser
    {
        public static async Task<BaseTableSchemaApiItemDto> GetFieldSchema(LarkClient larkClient, string appId, string tableId, string fieldId)
        {
            var tableSchemaRequest = new RestRequest($"bitable/v1/apps/{appId}/tables/{tableId}/fields", Method.Get);
            var tableSchemaResponse = await larkClient.ExecuteWithErrorHandling<BaseTableSchemaApiResponseDto>(tableSchemaRequest);
            var fieldSchema = tableSchemaResponse.Data.Items
                .FirstOrDefault(x => x.FieldId == fieldId)
                ?? throw new PluginMisconfigurationException($"Field with ID '{fieldId}' not found in table '{tableId}'.");

            return fieldSchema;
        }
    }
}
