using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
using Tests.Lark.Base;

namespace Tests.Lark;

[TestClass]
public class BaseTableTests : TestBase
{
    private BaseTableActions _baseTableActions => new(InvocationContext, FileManager);

    [TestMethod]
    public async Task SearchBaseTables_IssSuccess()
    {
        var response = await _baseTableActions.SearchBaseTables(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetBaseRecord_IssSuccess()
    {
        var response = await _baseTableActions.GetRecord(
            new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
            new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
            new GetBaseRecord { RecordID = "recuOXSfSwQlV8" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task UpdateBaseRecord_IssSuccess()
    {
        var response = await _baseTableActions.UpdateRecord(
            new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
            new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
            new UpdateRecordRequest
            {
                FieldName = "Custom text column name",
                NewValue="Hello my new value from upate action locally "
                //NewDateValue= DateTime.UtcNow.AddDays(2),
                //NewValues = new List<string> { "Option 12", "Option 21345435" },
                //Attachment = new FileReference { Name = "Test3.png" }
                //NewCheckboxValue = false
            },
            new GetBaseRecord { RecordID = "recuOXSfSwQlV8" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetBaseRecordPersonTypeEntry_IssSuccess()
    {
        var response = await _baseTableActions.GetPersonEntry(
            new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
            new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
            new GetBaseRecord { RecordID = "recQXFIkrm" },
            new GetPersonFieldRequest { FieldId = "fldqncxBMn" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetBaseRecordDateTypeEntry_IssSuccess()
    {
        var response = await _baseTableActions.GetDateEntries(
            new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
            new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
            new GetBaseRecord { RecordID = "recaqVFKCW" },
            new GetDateFieldRequest { FieldId = "fld3o9NPaH" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetBaseRecordTextTypeEntry_IssSuccess()
    {
        var response = await _baseTableActions.GetTextEntry(
           new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
           new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
           new GetBaseRecord { RecordID = "recaqVFKCW" },
           new GetTextFieldRequest { FieldId = "fldBAPISc0" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetMultiOptionValueFromRecord_IsSuccess()
    {
        var response = await _baseTableActions.GetMultiOptionValueFromRecord(
            new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
            new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
            new GetBaseRecord { RecordID = "recaqVFKCW" },
            new GetFieldRequest { FieldId = "fldlvpfJ7u" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetBaseRecordNumberTypeEntry_IssSuccess()
    {
        var response = await _baseTableActions.GetNumberEntry(
            new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
            new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
            new GetBaseRecord { RecordID = "recaqVFKCW" },
            new GetNumberFieldRequest { FieldId = "fldJUxetZw" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetBaseRecordFilesTypeEntry_IssSuccess()
    {
        var response = await _baseTableActions.DownloadAttachments(
            new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
            new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
            new GetBaseRecord { RecordID = "recaqVFKCW" },
            new GetDownloadFieldRequest { FieldId = "fldsZurxhF" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task GetBaseTableUsedRange_IssSuccess()
    {
        var response = await _baseTableActions.GetBaseRecords(
            new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
            new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" });

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
        Console.WriteLine(json);
        Assert.IsNotNull(response);
    }

    [TestMethod]
    public async Task InsertBaseTableRow_IssSuccess()
    {
        var baseRequest = new BaseRequest { AppId = "BqWJbD6KnaJpaMsj1JZjwekIpqx" };
        var tableRequest = new BaseTableRequest { TableId = "tblehXTNgiIpEOpA" };

        var response = await _baseTableActions.InsertBaseTableRow(baseRequest, tableRequest);

        Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented));
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RecordId));
    }
}
