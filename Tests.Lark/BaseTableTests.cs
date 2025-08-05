using Apps.Lark.Actions;
using Apps.Lark.Models.Request;
using Apps.Lark.Polling.Models;
using Apps.Lark.Webhooks;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Tests.Lark.Base;

namespace Tests.Lark
{
    [TestClass]
    public class BaseTableTests : TestBase
    {
        [TestMethod]
        public async Task SearchBaseTables_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.SearchBaseTables(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecord_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetRecord(
                new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recaqVFKCW" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task UpdateBaseRecord_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.UpdateRecord(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" },
                new UpdateRecordRequest
                {
                    FieldName = "Custom text column name",
                    NewValue="Hello + my new value from upate action locally "
                    //NewDateValue= DateTime.UtcNow.AddDays(2),
                    //NewValues = new List<string> { "Option 12", "Option 21345435" },
                    //Attachment = new FileReference { Name = "Test3.png" }
                },
                new GetBaseRecord { RecordID = "recuOXSfSwQlV8" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordPersonTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetPersonEntry(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recQXFIkrm" },
                new GetPersonFieldRequest { FieldId= "fldqncxBMn" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task GetBaseRecordDateTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetDateEntries(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recaqVFKCW" },
                new GetDateFieldRequest { FieldId= "fld3o9NPaH" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }


        [TestMethod]
        public async Task GetBaseRecordTextTypeEntry_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            //var response = await action.GetTextEntry(new BaseRequest { AppId = "Oacjbnzg3aMyAXsLgK5jR21Op0b" },
            //    new BaseTableRequest { TableId = "tblzSbOM8CQupYfE" },
            //    new GetBaseRecord { RecordID = "recuQsfE1GO90j" },
            //    new GetTextFieldRequest { FieldId= "fldKO35rlm" });

            var response = await action.GetTextEntry(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
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
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetMultiOptionValueFromRecord(
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
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetNumberEntry(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
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
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.DownloadAttachments(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" },
                new GetBaseRecord { RecordID = "recaqVFKCW" },
                new GetDownloadFieldRequest { FieldId= "fldsZurxhF" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }


        [TestMethod]
        public async Task GetBaseTableUsedRange_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            var response = await action.GetBaseRecords(new BaseRequest { AppId = "L1SebpqSKaRQccsJlybjAO4Bppg" },
                new BaseTableRequest { TableId = "tblJsOhO5AZt86JB" });

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }


        //InsertBaseTableRow

        [TestMethod]
        public async Task InsertBaseTableRow_IssSuccess()
        {
            var action = new BaseTableActions(InvocationContext, FileManager);
            await action.InsertBaseTableRow(new BaseRequest { AppId = "MXjZb5uHvahFiMs5mUvjIzC9pxf" },
                new BaseTableRequest { TableId = "tblORLQK2OUtTZ9p" });

            Assert.IsTrue(true);
        }


        [TestMethod]
        public async Task Webhook_Test()
        {
            var action = new WebhookList(InvocationContext);

            var jsonPayload = @"{
                ""schema"": ""2.0"",
                ""header"": {
                    ""event_id"": ""5e3702a84e847582be8db7fb73283c02"",
                    ""event_type"": ""drive.file.bitable_record_changed_v1"",
                    ""create_time"": ""1608725989000"",
                    ""token"": ""rvaYgkND1GOiu5MM0E1rncYC6PLtF7JV"",
                    ""app_id"": ""cli_9f5343c580712544"",
                    ""tenant_key"": ""2ca1d211f64f6438""
                },
                ""event"": {
                    ""file_type"": ""bitable"",
                    ""file_token"": ""bascnItn6oHUSEL8RDUdF6abcef"",
                    ""table_id"": ""tblOaqBWfGeabcef"",
                    ""revision"": 41,
                    ""operator_id"": {
                        ""union_id"": ""on_8ed6aa67826108097d9ee143816345"",
                        ""user_id"": ""e33ggbyz"",
                        ""open_id"": ""ou_84aad35d084aa403a838cf73ee18467""
                    },
                    ""action_list"": [
                        {
                            ""record_id"": ""rec9sabcef"",
                            ""action"": ""record_edited"",
                            ""before_value"": [
                                {
                                    ""field_id"": ""fld9Eabcef"",
                                    ""field_value"": ""666"",
                                    ""field_identity_value"": {
                                        ""users"": [
                                            {
                                                ""user_id"": {
                                                    ""union_id"": ""on_8ed6aa67826108097d9ee143816345"",
                                                    ""user_id"": ""e33ggbyz"",
                                                    ""open_id"": ""ou_84aad35d084aa403a838cf73ee18467""
                                                },
                                                ""name"": ""张敏"",
                                                ""en_name"": ""Zhangmin"",
                                                ""avatar_url"": ""https://internal-api-lark-file.larksuite.com/static-resource/v1/v2_q86-fcb6-4f18-85c7-87ca8881e50j~?image_size=72x72&cut_type=default-face&quality=&format=jpeg&sticker_format=.webp""
                                            }
                                        ]
                                    }
                                }
                            ],
                            ""after_value"": [
                                {
                                    ""field_id"": ""fld9Eabcef"",
                                    ""field_value"": ""666"",
                                    ""field_identity_value"": {
                                        ""users"": [
                                            {
                                                ""user_id"": {
                                                    ""union_id"": ""on_8ed6aa67826108097d9ee143816345"",
                                                    ""user_id"": ""e33ggbyz"",
                                                    ""open_id"": ""ou_84aad35d084aa403a838cf73ee18467""
                                                },
                                                ""name"": ""张敏"",
                                                ""en_name"": ""Zhangmin"",
                                                ""avatar_url"": ""https://internal-api-lark-file.larksuite.com/static-resource/v1/v2_q86-fcb6-4f18-85c7-87ca8881e50j~?image_size=72x72&cut_type=default-face&quality=&format=jpeg&sticker_format=.webp""
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                    ],
                    ""subscriber_id_list"": [
                        {
                            ""union_id"": ""on_8ed6aa67826108097d9ee143816345"",
                            ""user_id"": ""638474b8"",
                            ""open_id"": ""ou_9bc587355789fc049904ae7c73619b89""
                        }
                    ],
                    ""update_time"": 1717040601
                }
            }";

            var webhookRequest = new WebhookRequest
            {
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } },
                HttpMethod = HttpMethod.Post,
                Body = jsonPayload, 
                Url = "http://localhost/webhook",
                QueryParameters = new Dictionary<string, string>()
            };

            var response = await action.OnBaseTableRecordUpdated(webhookRequest, new BaseTableFiltersRequest { });
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(response, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
            Assert.IsNotNull(response);
        }
    }
}
