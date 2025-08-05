using Apps.Lark.Constants;
using Apps.Lark.Models.Dtos;
using Newtonsoft.Json.Linq;

namespace Apps.Lark.Utils;
public static class BaseRecordJsonParser
{
    public static BaseRecordDto? ConvertToRecord(string json, IDictionary<string, BaseTableSchemaApiItemDto> schemaByFieldName)
    {
        var jObj = JObject.Parse(json);
        var recordToken = jObj["data"]?["record"];

        if (recordToken == null || recordToken.Type != JTokenType.Object)
            return null;

        var rawRecord = (JObject)recordToken;
        if (rawRecord == null)
            return null;

        return ConvertToBaseRecord(rawRecord, schemaByFieldName);
    }

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

        var fields = new List<BaseRecordFieldListItemDto>();

        foreach (var schema in schemaByFieldName.Values)
        {
            string? value = null;

            if (fieldsPayload != null
                && fieldsPayload.TryGetValue(schema.FieldName, out var token)
                && token != null)
            {
                value = ConvertFieldToString(token, schema.FieldTypeId);
            }

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
        if (field == null || field.Type == JTokenType.Null)
            return string.Empty;

        if (field.Type == JTokenType.String)
        {
            var raw = field.Value<string>() ?? string.Empty;
            if (raw.TrimStart().StartsWith("["))
            {
                try
                {
                    var arr = JArray.Parse(raw);
                    return fieldType switch
                    {
                        BaseFieldTypes.Multiline
                      or BaseFieldTypes.MultipleOptions
                      or BaseFieldTypes.Person
                      or BaseFieldTypes.CreatedBy
                      or BaseFieldTypes.ModifiedBy
                      or BaseFieldTypes.Attachment
                      or BaseFieldTypes.GroupChat => StringFromArray(", ", arr, "text"),

                        BaseFieldTypes.Lookup => StringFromArray(", ", (arr.First?["value"] as JArray)),
                        BaseFieldTypes.OneWayLink
                      or BaseFieldTypes.TwoWayLink => StringFromRelation(arr as JToken),

                        _ => raw
                    };
                }
                catch
                {
                    return raw;
                }
            }
            return raw;
        }


        return fieldType switch
        {
            BaseFieldTypes.Multiline => field.Type == JTokenType.String
                ? field.Value<string>() ?? string.Empty
                : StringFromArray(string.Empty, field as JArray, "text"),
            BaseFieldTypes.MultipleOptions => StringFromArray(", ", field as JArray),
            BaseFieldTypes.Person => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.CreatedBy => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.ModifiedBy => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.Attachment => StringFromArray(", ", field as JArray, "name"),
            BaseFieldTypes.Lookup => StringFromArray(", ", field["value"] as JArray),
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

            BaseFieldTypes.Formula => StringFromFormula(field),

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
        if (token == null || token.Type == JTokenType.Null || (token.Type != JTokenType.Integer && token.Type != JTokenType.Float))
            return string.Empty;

        var value = token.Value<long?>();
        if (!value.HasValue)
            return string.Empty;

        var offset = DateTimeOffset.FromUnixTimeMilliseconds(value.Value);
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

    private static string StringFromFormula(JToken field)
    {
        if (field.Type == JTokenType.Array)
        {
            return StringFromArray(string.Empty, field as JArray, "text");
        }

        if (field.Type != JTokenType.Object)
        {
            return field.Value<string>() ?? string.Empty;
        }

        // Lark returns list fields as is,
        // but converts other types to lists with a single item
        var listFieldTypes = new List<int>
        {
            BaseFieldTypes.Multiline,
            BaseFieldTypes.MultipleOptions,
            BaseFieldTypes.Person,
            BaseFieldTypes.CreatedBy,
            BaseFieldTypes.ModifiedBy,
            BaseFieldTypes.Attachment,
            BaseFieldTypes.GroupChat,
        };

        var formulaResultFieldType = field["type"]?.Value<int>() ?? -1;

        var formulaResultFieldValue = listFieldTypes.Contains(formulaResultFieldType)
            ? field["value"]
            : field["value"]?[0];

        if (formulaResultFieldValue == null || formulaResultFieldType == -1)
        {
            return string.Empty;
        }

        return ConvertFieldToString(formulaResultFieldValue, formulaResultFieldType);
    }
}
