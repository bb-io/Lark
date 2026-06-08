using Apps.Lark.Utils;
using Newtonsoft.Json.Linq;

namespace Tests.Lark;

[TestClass]
public class LarkOutputValueNormalizerTests
{
    [TestMethod]
    public void NormalizeDictionary_ConvertsNewtonsoftTypesToPlainClrValues()
    {
        var raw = new Dictionary<string, object>
        {
            ["Text"] = "test from bb.io",
            ["Number"] = JToken.Parse("12"),
            ["Checkbox"] = JToken.Parse("true"),
            ["Date"] = JToken.Parse("1751705570567"),
            ["Link"] = JToken.Parse(@"{""link"":""https://example.com"",""text"":""https://example.com""}"),
            ["Attachment"] = JToken.Parse(@"[{""file_token"":""abc"",""name"":""image.png""}]"),
            ["Multi"] = JToken.Parse(@"[""a"",""b""]")
        };

        var normalized = LarkOutputValueNormalizer.NormalizeDictionary(raw);

        Assert.AreEqual("test from bb.io", normalized["Text"]);
        Assert.AreEqual(12L, normalized["Number"]);
        Assert.AreEqual(true, normalized["Checkbox"]);
        Assert.AreEqual(1751705570567L, normalized["Date"]);
        Assert.IsInstanceOfType<Dictionary<string, object>>(normalized["Link"]);
        Assert.IsInstanceOfType<List<object?>>(normalized["Attachment"]);
        Assert.IsInstanceOfType<List<object?>>(normalized["Multi"]);
        Assert.IsFalse(ContainsNewtonsoftToken(normalized));
    }

    [TestMethod]
    public void StringifyValue_FormatsCollectionsForUi()
    {
        var normalized = new Dictionary<string, object>
        {
            ["Link"] = new Dictionary<string, object>
            {
                ["link"] = "https://example.com",
                ["text"] = "Example"
            },
            ["Attachment"] = new List<object?>
            {
                new Dictionary<string, object>
                {
                    ["file_token"] = "abc",
                    ["name"] = "image.png"
                }
            },
            ["Multi"] = new List<object?> { "a", "b" }
        };

        Assert.AreEqual("https://example.com, Example", LarkOutputValueNormalizer.StringifyValue(normalized["Link"]));
        Assert.AreEqual("abc, image.png", LarkOutputValueNormalizer.StringifyValue(normalized["Attachment"]));
        Assert.AreEqual("a, b", LarkOutputValueNormalizer.StringifyValue(normalized["Multi"]));
    }

    private static bool ContainsNewtonsoftToken(object? value)
    {
        if (value == null)
        {
            return false;
        }

        if (value is JToken)
        {
            return true;
        }

        if (value is IDictionary<string, object> dictionary)
        {
            return dictionary.Values.Any(ContainsNewtonsoftToken);
        }

        if (value is IEnumerable<object?> items)
        {
            return items.Any(ContainsNewtonsoftToken);
        }

        return false;
    }
}
