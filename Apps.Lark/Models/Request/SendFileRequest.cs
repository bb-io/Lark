using Apps.Appname.Handlers.Static;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Lark.Models.Request
{
    public class SendFileRequest
    {
        public string ReceiveId { get; set; }

        [StaticDataSource(typeof(ReceiveIdTypeHandler))]
        public string ReceiveIdType { get; set; }

        public FileReference FileContent { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string FileType { get; set; }
    }
}
