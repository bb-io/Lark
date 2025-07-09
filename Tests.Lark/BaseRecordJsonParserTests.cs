using Apps.Lark.Constants;
using Apps.Lark.Utils;
using Newtonsoft.Json.Linq;
using Tests.Lark.Base;

namespace Tests.Lark;

[TestClass]
public class BaseRecordJsonParserTests : TestBase
{
    private readonly JObject _sampleRecordFields;

    public BaseRecordJsonParserTests()
    {
        var json = @"{
          ""EN Status"": ""Not yet"",
          ""Figma"": {
            ""link"": ""https://docs.blackbird.io/apps/lark/"",
            ""text"": ""Localization requests""
          },
          ""Keys"": [
            { ""text"": ""\""key\"": \""value\"", Hello world!\n"" },
            { ""text"": ""\""example_1\"": \""Banner\"", 10,\n"" },
            { ""text"": ""\""example_2\"": \""Longer keys\"", 25"" }
          ],
          ""Languages"": [ ""All languages"", ""tr"" ],
          ""Requestor"": [
            { ""id"": ""ou_dbbbe063d6c69e470506f249b5cecbd8"", ""name"": ""Alex Terekhov"" }
          ],
          ""Screenshots"": [
            { ""file_token"": ""VRNRbmAEuoX7RTxFuUIjephPpze"", ""name"": ""walpaper.jpg"" },
            { ""file_token"": ""BTOBb2YlAopvEoxRHmajFPMqpkb"", ""name"": ""blackbird-icon.png"" }
          ],
          ""Submitted on"": 1751705570567,
          ""Checkbox"": true,
          ""Created by"": [
            { ""id"": ""ou_dbbbe063d6c69e470506f249b5cecbd8"", ""name"": ""Alex Terekhov"" }
          ],
          ""Date created"": 1751705570000,
          ""Date modified"": 1751837133000,
          ""Group chat"": [
            { ""id"": ""oc_609f2890ea52c8590f756ef79f8240c7"", ""name"": ""Test company - blackbird-demo"" }
          ],
          ""Location"": { ""full_address"": ""St. John's, NL"" },
          ""Lookup"": { ""value"": [ ""Web"" ] },
          ""Phone Number"": ""+18079999999"",
          ""Modified by"": [
            { ""id"": ""ou_dbbbe063d6c69e470506f249b5cecbd8"", ""name"": ""Alex Terekhov"" }
          ],
          ""Number"": 10,
          ""One-way link"": { ""link_record_ids"": [ ""recEggw9gh"", ""recpBd91WM"" ] },
          ""Two-way link"": { ""link_record_ids"": [ ""recmCOhl6k"", ""recVsEd4yv"" ] },
          ""Request ID"": { ""value"": [ { ""text"": ""recTdFEeh5"" } ] }
        }";
        _sampleRecordFields = JObject.Parse(json);
    }

    [TestMethod]
    public void ConvertFieldToString_Multiline_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Keys"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Multiline);
        Assert.AreEqual("\"key\": \"value\", Hello world!\n\"example_1\": \"Banner\", 10,\n\"example_2\": \"Longer keys\", 25", result);
    }

    [TestMethod]
    public void ConvertFieldToString_MultipleOptions_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Languages"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.MultipleOptions);
        Assert.AreEqual("All languages, tr", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Person_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Requestor"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Person);
        Assert.AreEqual("Alex Terekhov", result);
    }

    [TestMethod]
    public void ConvertFieldToString_CreatedBy_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Created by"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.CreatedBy);
        Assert.AreEqual("Alex Terekhov", result);
    }

    [TestMethod]
    public void ConvertFieldToString_ModifiedBy_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Modified by"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.ModifiedBy);
        Assert.AreEqual("Alex Terekhov", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Attachment_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Screenshots"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Attachment);
        Assert.AreEqual("walpaper.jpg, blackbird-icon.png", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Lookup_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Lookup"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Lookup);
        Assert.AreEqual("Web", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Formula_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Request ID"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Formula);
        Assert.AreEqual("recTdFEeh5", result);
    }

    [TestMethod]
    public void ConvertFieldToString_GroupChat_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Group chat"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.GroupChat);
        Assert.AreEqual("Test company - blackbird-demo", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Date_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Submitted on"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Date);
        Assert.AreEqual("2025-07-05T08:52:50.5670000Z", result);
    }

    [TestMethod]
    public void ConvertFieldToString_DateCreated_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Date created"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.DateCreated);
        Assert.AreEqual("2025-07-05T08:52:50.0000000Z", result);
    }

    [TestMethod]
    public void ConvertFieldToString_LastModifiedDate_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Date modified"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.LastModifiedDate);
        Assert.AreEqual("2025-07-06T21:25:33.0000000Z", result);
    }

    [TestMethod]
    public void ConvertFieldToString_OneWayLink_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["One-way link"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.OneWayLink);
        Assert.AreEqual("recEggw9gh, recpBd91WM", result);
    }

    [TestMethod]
    public void ConvertFieldToString_OneWayWayLink_ReturnsCorrectStringFromDetailedInput()
    {
        var json = @"{
          ""One-way link"": [
          {
            ""record_ids"": [
              ""recmCOhl6k"",
              ""recVsEd4yv""
            ],
            ""table_id"": ""tblzSbOM8CQupYfE"",
            ""text"": ""2025/06/20,2025/06/20"",
            ""text_arr"": [
              ""2025/06/20"",
              ""2025/06/20""
            ],
            ""type"": ""text""
          }
        ]
        }";
        var sampleRecordFields = JObject.Parse(json);
        var field = sampleRecordFields["One-way link"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.TwoWayLink);
        Assert.AreEqual("recmCOhl6k, recVsEd4yv", result);
    }

    [TestMethod]
    public void ConvertFieldToString_TwoWayLink_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Two-way link"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.TwoWayLink);
        Assert.AreEqual("recmCOhl6k, recVsEd4yv", result);
    }

    [TestMethod]
    public void ConvertFieldToString_TwoWayLink_ReturnsCorrectStringFromDetailedInput()
    {
        var json = @"{
          ""Two-way link"": [
          {
            ""record_ids"": [
              ""recmCOhl6k"",
              ""recVsEd4yv""
            ],
            ""table_id"": ""tblzSbOM8CQupYfE"",
            ""text"": ""2025/06/20,2025/06/20"",
            ""text_arr"": [
              ""2025/06/20"",
              ""2025/06/20""
            ],
            ""type"": ""text""
          }
        ]
        }";
        var sampleRecordFields = JObject.Parse(json);
        var field = sampleRecordFields["Two-way link"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.TwoWayLink);
        Assert.AreEqual("recmCOhl6k, recVsEd4yv", result);
    }

    [TestMethod]
    public void ConvertFieldToString_PhoneNumber_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Phone Number"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.PhoneNumber);
        Assert.AreEqual("+18079999999", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Number_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Number"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Number);
        Assert.AreEqual("10", result);
    }

    [TestMethod]
    public void ConvertFieldToString_SingleOption_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["EN Status"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.SingleOption);
        Assert.AreEqual("Not yet", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Checkbox_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Checkbox"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Checkbox);
        Assert.AreEqual("True", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Link_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Figma"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Link);
        Assert.AreEqual("https://docs.blackbird.io/apps/lark/", result);
    }

    [TestMethod]
    public void ConvertFieldToString_Location_ReturnsCorrectString()
    {
        var field = _sampleRecordFields["Location"] ?? throw new Exception("Sample filed was not found.");
        var result = BaseRecordJsonParser.ConvertFieldToString(field, BaseFieldTypes.Location);
        Assert.AreEqual("St. John's, NL", result);
    }
}
