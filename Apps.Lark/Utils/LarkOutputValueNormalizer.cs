using System.Collections;
using Newtonsoft.Json.Linq;

namespace Apps.Lark.Utils;

public static class LarkOutputValueNormalizer
{
    public static Dictionary<string, object> NormalizeDictionary(IDictionary<string, object>? fields)
    {
        var normalized = new Dictionary<string, object>();

        if (fields == null)
        {
            return normalized;
        }

        foreach (var field in fields)
        {
            normalized[field.Key] = NormalizeValue(field.Value)!;
        }

        return normalized;
    }

    public static object? NormalizeValue(object? value)
    {
        return value switch
        {
            null => null,
            JValue jValue => jValue.Value,
            JObject jObject => NormalizeJObject(jObject),
            JArray jArray => NormalizeJArray(jArray),
            JToken jToken => NormalizeToken(jToken),
            IDictionary<string, object> dictionary => NormalizeDictionary(dictionary),
            IDictionary dictionary => NormalizeNonGenericDictionary(dictionary),
            IEnumerable enumerable when value is not string => NormalizeEnumerable(enumerable),
            _ => value
        };
    }

    private static object? NormalizeToken(JToken token)
    {
        return token switch
        {
            JValue jValue => jValue.Value,
            JObject jObject => NormalizeJObject(jObject),
            JArray jArray => NormalizeJArray(jArray),
            _ => token.ToString()
        };
    }

    private static Dictionary<string, object> NormalizeJObject(JObject value)
    {
        var normalized = new Dictionary<string, object>();

        foreach (var property in value.Properties())
        {
            normalized[property.Name] = NormalizeToken(property.Value)!;
        }

        return normalized;
    }

    private static List<object?> NormalizeJArray(JArray value)
    {
        return value.Select(NormalizeToken).ToList();
    }

    private static Dictionary<string, object> NormalizeNonGenericDictionary(IDictionary value)
    {
        var normalized = new Dictionary<string, object>();

        foreach (DictionaryEntry entry in value)
        {
            if (entry.Key is string key)
            {
                normalized[key] = NormalizeValue(entry.Value)!;
            }
        }

        return normalized;
    }

    private static List<object?> NormalizeEnumerable(IEnumerable value)
    {
        var normalized = new List<object?>();

        foreach (var item in value)
        {
            normalized.Add(NormalizeValue(item));
        }

        return normalized;
    }
}
