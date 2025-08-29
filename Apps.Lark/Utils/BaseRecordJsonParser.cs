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
            var trimmed = raw.TrimStart();
            if ((trimmed.StartsWith("[") && trimmed.EndsWith("]")) ||
                (trimmed.StartsWith("{") && trimmed.EndsWith("}")))
            {
                try
                {
                    JToken parsed = trimmed.StartsWith("[")
                        ? (JToken)JArray.Parse(raw)
                        : JObject.Parse(raw);

                    return ConvertFieldToString(parsed, fieldType);
                }
                catch
                {
                    field = JValue.CreateString(raw);
                }
            }
            else
            {
                field = JValue.CreateString(raw);
            }
        }

        return fieldType switch
        {
            BaseFieldTypes.Multiline => field.Type == JTokenType.String
                ? field.Value<string>() ?? string.Empty
                : StringFromArray(string.Empty, field as JArray, "text"),

            BaseFieldTypes.MultipleOptions => field.Type == JTokenType.Array
                ? StringFromArray(", ", field as JArray)
                : field.ToString(),

            BaseFieldTypes.Person => ExtractPersonName(field),
            BaseFieldTypes.CreatedBy => ExtractPersonName(field),
            BaseFieldTypes.ModifiedBy => ExtractPersonName(field),
            BaseFieldTypes.GroupChat => ExtractPersonName(field),

            BaseFieldTypes.Attachment => StringFromArray(", ", field as JArray, "name"),

            BaseFieldTypes.Lookup => field.Type switch
            {
                JTokenType.Array => StringFromArray(", ", field as JArray),
                JTokenType.Object => (field["value"] is JArray ja)
                    ? StringFromArray(", ", ja)
                    : (field["value"]?.ToString() ?? string.Empty),
                _ => field.Value<string>() ?? string.Empty
            },

            BaseFieldTypes.Date => StringFromTimestamp(field),
            BaseFieldTypes.DateCreated => StringFromTimestamp(field),
            BaseFieldTypes.LastModifiedDate => StringFromTimestamp(field),

            BaseFieldTypes.OneWayLink => StringFromRelation(field),
            BaseFieldTypes.TwoWayLink => StringFromRelation(field),

            BaseFieldTypes.Number => field.Value<string>() ?? string.Empty,
            BaseFieldTypes.SingleOption => field.Value<string>() ?? string.Empty,
            BaseFieldTypes.PhoneNumber => field.Value<string>() ?? string.Empty,

            BaseFieldTypes.Checkbox => field.Value<bool>() ? "True" : "False",

            BaseFieldTypes.Link => field.Type == JTokenType.Object
                ? (field["link"]?.Value<string>() ?? field["text"]?.Value<string>() ?? string.Empty)
                : (field.Value<string>() ?? string.Empty),

            BaseFieldTypes.Location => field.Type == JTokenType.Object
                ? (field["full_address"]?.Value<string>() ?? string.Empty)
                : (field.Value<string>() ?? string.Empty),

            BaseFieldTypes.Formula => StringFromFormula(field),

            _ => string.Empty
        };
    }

    private static string ExtractPersonName(JToken t)
    {
        if (t == null || t.Type == JTokenType.Null) return string.Empty;

        if (t is JArray arr)
        {
            var names = arr.Select(ExtractPersonName)
                           .Where(s => !string.IsNullOrEmpty(s));
            return string.Join(", ", names);
        }

        if (t is JObject o)
        {
            if (o.TryGetValue("users", out var usersTok) && usersTok is JArray usersArr)
                return ExtractPersonName(usersArr);

            return o["name"]?.ToString()
                ?? o["en_name"]?.ToString()
                ?? o["enName"]?.ToString()
                ?? o["email"]?.ToString()
                ?? string.Empty;
        }

        return t.Value<string>() ?? string.Empty;
    }

    private static string StringFromArray(string joinBy, JArray? arr, string property)
    {
        if (arr == null || arr.Count == 0)
            return string.Empty;

        var values = new List<string>();

        foreach (var t in arr)
        {
            if (t == null || t.Type == JTokenType.Null)
                continue;

            switch (t.Type)
            {
                case JTokenType.Object:
                    {
                        var obj = (JObject)t;
                        var v = obj[property]?.ToString();

                        if (string.IsNullOrEmpty(v))
                            v = obj["text"]?.ToString()
                                ?? obj["name"]?.ToString()
                                ?? obj["value"]?.ToString();

                        if (!string.IsNullOrEmpty(v))
                            values.Add(v);
                        break;
                    }
                case JTokenType.Array:
                    {
                        var nested = StringFromArray(joinBy, (JArray)t, property);
                        if (!string.IsNullOrEmpty(nested))
                            values.Add(nested);
                        break;
                    }
                default:
                    {
                        var s = t.ToString();
                        if (!string.IsNullOrEmpty(s))
                            values.Add(s);
                        break;
                    }
            }
        }

        return string.Join(joinBy, values.Where(x => !string.IsNullOrEmpty(x)));
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
