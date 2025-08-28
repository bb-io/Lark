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
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "U20EbzMzSaRz3psPAsBlqNEOgZd" };

        // Execute
        var response = await _baseTableActions.SearchBaseTables(baseRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetBaseRecord_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "U20EbzMzSaRz3psPAsBlqNEOgZd" };
        var tableRequest = new BaseTableRequest { TableId = "tblmJb3RMxHfMFbd" };
        var recordRequest = new GetBaseRecord { RecordID = "recvd08jOE" };

        // Execute
        var response = await _baseTableActions.GetRecord(baseRequest, tableRequest, recordRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task UpdateBaseRecord_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" };
        var tableRequest = new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" };
        var recordRequest = new GetBaseRecord { RecordID = "recuOXSfSwQlV8" };
        var updateRequest = new UpdateRecordRequest
        {
            FieldName = "Custom text column name",
            NewValue = "Hello my new value from upate action locally "
            //NewDateValue= DateTime.UtcNow.AddDays(2),
            //NewValues = new List<string> { "Option 12", "Option 21345435" },
            //Attachment = new FileReference { Name = "Test3.png" }
            //NewCheckboxValue = false
        };

        // Execute
        var response = await _baseTableActions.UpdateRecord(baseRequest, tableRequest, updateRequest, recordRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetBaseRecordPersonTypeEntry_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" };
        var tableRequest = new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" };
        var recordRequest = new GetBaseRecord { RecordID = "recQXFIkrm" };
        var fieldRequest = new GetPersonFieldRequest { FieldId = "fldqncxBMn" };

        // Execute
        var response = await _baseTableActions.GetPersonEntry(baseRequest, tableRequest, recordRequest, fieldRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetBaseRecordDateTypeEntry_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" };
        var tableRequest = new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" };
        var recordRequest = new GetBaseRecord { RecordID = "recaqVFKCW" };
        var fieldRequest = new GetDateFieldRequest { FieldId = "fld3o9NPaH" };

        // Execute
        var response = await _baseTableActions.GetDateEntries(baseRequest, tableRequest, recordRequest, fieldRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetBaseRecordTextTypeEntry_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" };
        var tableRequest = new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" };
        var recordRequest = new GetBaseRecord { RecordID = "recaqVFKCW" };
        var fieldRequest = new GetTextFieldRequest { FieldId = "fldBAPISc0" };

        // Execute
        var response = await _baseTableActions.GetTextEntry(baseRequest, tableRequest, recordRequest, fieldRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetMultiOptionValueFromRecord_IsSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" };
        var tableRequest = new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" };
        var recordRequest = new GetBaseRecord { RecordID = "recaqVFKCW" };
        var fieldRequest = new GetFieldRequest { FieldId = "fldlvpfJ7u" };

        // Execute
        var response = await _baseTableActions.GetMultiOptionValueFromRecord(baseRequest, tableRequest, recordRequest, fieldRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetBaseRecordNumberTypeEntry_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" };
        var tableRequest = new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" };
        var recordRequest = new GetBaseRecord { RecordID = "recaqVFKCW" };
        var fieldRequest = new GetNumberFieldRequest { FieldId = "fldJUxetZw" };

        // Execute
        var response = await _baseTableActions.GetNumberEntry(baseRequest, tableRequest, recordRequest, fieldRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetBaseRecordFilesTypeEntry_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" };
        var tableRequest = new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" };
        var recordRequest = new GetBaseRecord { RecordID = "recaqVFKCW" };
        var fieldRequest = new GetDownloadFieldRequest { FieldId = "fldsZurxhF" };

        // Execute
        var response = await _baseTableActions.DownloadAttachments(baseRequest, tableRequest, recordRequest, fieldRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task GetBaseTableUsedRange_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" };
        var tableRequest = new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" };

        // Execute
        var response = await _baseTableActions.GetBaseRecords(baseRequest, tableRequest);

        // Print result and assert
        PrintResult(response);
    }

    [TestMethod]
    public async Task InsertBaseTableRow_IssSuccess()
    {
        // Setup parameters
        var baseRequest = new BaseRequest { AppId = "BqWJbD6KnaJpaMsj1JZjwekIpqx" };
        var tableRequest = new BaseTableRequest { TableId = "tblehXTNgiIpEOpA" };

        // Execute
        var response = await _baseTableActions.InsertBaseTableRow(baseRequest, tableRequest);

        // Print result and assert
        PrintResult(response);
        Assert.IsFalse(string.IsNullOrWhiteSpace(response.RecordId));
    }
}
