using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GraphSdkDemo.SharePoint.Api.Contracts.File
{
    [DataContract]
    public class DownloadFilesInputModel
    {
        [DataMember(Name = "driveItems")]
        public IEnumerable<string> DriveItems { get; set; }
    }
}
