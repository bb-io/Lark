using Blackbird.Applications.Sdk.Common;

namespace Apps.Lark.Models.Request
{
    public class BaseRequest
    {
        /// <summary>
        /// This has to be a string provided by the user
        /// It's not possible to have a data handler for a Base:
        /// - `/open-apis/drive/v1/files` endpoint is only looking for bases created by the app
        /// - `/open-apis/suite/docs-api/search/object` endpoint is only returning bases if at least one character provided for a search string
        /// 
        /// Base ID can be obtained from the URL:           vvvvvvvvvvvvvvvvvvvvvvvvvvv
        /// https://test-db2hx0p0w6r1.jp.larksuite.com/base/Oacjbnzg3aMyAXsLgK5jR21Op0b?table=tblzSbOM8CQupYfE&view=vewPhU1Ooh
        ///                                                 ^^^^^^^^^^^^^^^^^^^^^^^^^^^
        /// </summary>
        [Display("Base ID")]
        public string AppId { get; set; } = string.Empty;
    }
}
