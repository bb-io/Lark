using Apps.Lark.Constants;
using Apps.Lark.Models.Dtos;
using Newtonsoft.Json.Linq;

namespace Apps.Lark.Utils;
public static class BaseRecordJsonParser
{
    public static IEnumerable<BaseRecordDto> ConvertToRecordsList(string json, IDictionary<string, BaseTableSchemaApiItemDto> schemaByFieldName)
    {
        var jObj = JObject.Parse(json);

        var records = jObj["data"]?["items"] as JArray;
        if (records == null || records.Count == 0)
            yield break;

        foreach (var recordToken in records)
        {
            if (recordToken == null || recordToken.Type != JTokenType.Object)
                continue;

            var rawRecord = (JObject)recordToken;
            if (rawRecord == null)
                continue;

            var parsedRecord = ConvertToBaseRecord(rawRecord, schemaByFieldName);
            if (parsedRecord != null)
                yield return parsedRecord;
        }
    }

    public static BaseRecordDto? ConvertToBaseRecord(JObject record, IDictionary<string, BaseTableSchemaApiItemDto> schemaByFieldName)
    {
        if (record?.ContainsKey("record_id") != true)
            return null;

        var recordId = record["record_id"]?.ToString() ?? string.Empty;
        if (string.IsNullOrEmpty(recordId))
            return null;

        var fieldsPayload = record["fields"] as JObject;
        if (fieldsPayload == null)
            return null;

        var fields = new List<BaseRecordFieldListItemDto>();

        foreach (var prop in fieldsPayload.Properties())
        {
            if (!schemaByFieldName.TryGetValue(prop.Name, out var schema))
                continue; // skip unknown fields

            var fieldToken = prop.Value;
            if (fieldToken == null)
                continue;

            string value = ConvertFieldToString(fieldToken, schema.FieldTypeId);

            fields.Add(new BaseRecordFieldListItemDto
            {
                FieldId = schema.FieldId,
                FieldName = schema.FieldName,
                FieldType = schema.FieldTypeName,
                FieldValue = value
            });
        }

        return new BaseRecordDto
        {
            RecordId = recordId,
            Fields = fields
        };
    }

    public static string ConvertFieldToString(JToken field, int fieldType)
    {
        return fieldType switch
        {
            BaseFieldTypes.Multiline => StringFromArray(string.Empty, field as JArray, "text"),
            BaseFieldTypes.MultipleOptions => StringFromArray(", ", field as JArray),
            BaseFieldTypes.Person => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.CreatedBy => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.ModifiedBy => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.Attachment => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.Lookup => StringFromArray(", ", field["value"] as JArray),
            BaseFieldTypes.Formula => StringFromArray(", ", field["value"] as JArray, "text"),
            BaseFieldTypes.GroupChat => StringFromArray(", ", field as JArray, "name"),

            BaseFieldTypes.Date => StringFromTimestamp(field),
            BaseFieldTypes.DateCreated => StringFromTimestamp(field),
            BaseFieldTypes.LastModifiedDate => StringFromTimestamp(field),

            BaseFieldTypes.OneWayLink => StringFromRelation(field),
            BaseFieldTypes.TwoWayLink => StringFromRelation(field),

            BaseFieldTypes.Number => field.Value<string>() ?? string.Empty,
            BaseFieldTypes.SingleOption => field.Value<string>() ?? string.Empty,
            BaseFieldTypes.PhoneNumber => field.Value<string>() ?? string.Empty,

            BaseFieldTypes.Checkbox => field.Value<bool>() ? "True" : "False",

            BaseFieldTypes.Link => field?["link"]?.Value<string>() ?? string.Empty,

            BaseFieldTypes.Location => field?["full_address"]?.Value<string>() ?? string.Empty,

            _ => string.Empty
        };
    }

    private static string StringFromArray(string joinBy, JArray? arr, string property)
    {
        if (arr == null || arr.Count == 0)
            return string.Empty;

        var values = arr
            .Select(t => t[property]?.ToString() ?? string.Empty)
            .Where(v => !string.IsNullOrEmpty(v))
            .ToList();

        return string.Join(joinBy, values);
    }

    private static string StringFromArray(string joinBy, JArray? arr)
    {
        if (arr == null || arr.Count == 0)
            return string.Empty;

        var values = arr
            .Select(t => t.ToString() ?? string.Empty)
            .Where(v => !string.IsNullOrEmpty(v))
            .ToList();

        return string.Join(joinBy, values);
    }

    private static string StringFromTimestamp(JToken token)
    {
        var offset = DateTimeOffset.FromUnixTimeMilliseconds(token.Value<long>());
        return offset.UtcDateTime.ToString("o");
    }

    private static string StringFromRelation(JToken field)
    {
        // search request returns only related ids
        if (field.Type == JTokenType.Object)
        {
            var linkFieldFromSearch = field?["link_record_ids"] as JArray;
            List<string> linkedIds = linkFieldFromSearch?.Select(t => t.ToString()).ToList() ?? [];
            return string.Join(", ", linkedIds);
        }

        // detailed record request returns array of full link details
        var linkField = field as JArray;
        List<string> recordIds = [];
        foreach (JToken? linkToken in linkField ?? [])
        {
            if (linkToken == null || linkToken.Type != JTokenType.Object)
                continue;
            var linkObject = (JObject)linkToken;
            if (linkObject.TryGetValue("record_ids", out JToken? idsToken))
            {
                if (idsToken == null || idsToken.Type != JTokenType.Array)
                    continue;
                foreach (JToken idToken in idsToken)
                {
                    recordIds.Add(idToken.ToString());
                }
            }
        }
        return string.Join(", ", recordIds);
    }
}
