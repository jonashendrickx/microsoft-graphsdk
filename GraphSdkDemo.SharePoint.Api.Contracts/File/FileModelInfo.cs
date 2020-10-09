using System;
using System.Runtime.Serialization;

namespace GraphSdkDemo.SharePoint.Api.Contracts.File
{
    [DataContract]
    public class FileModelInfo
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "size")]
        public long? Size { get; set; }

        [DataMember(Name = "created_at")]
        public DateTime? CreatedAt { get; set; }

        [DataMember(Name = "last_modified_at")]
        public DateTime? LastModifiedAt { get; set; }

        [DataMember(Name = "download_url")]
        public string DownloadUrl { get; set; }

        public static FileModelInfo FromView(SharePoint.Contracts.Files.File file)
        {
            var model = new FileModelInfo
            {
                Id = file?.Id,
                Name = file?.Name,
                Description = file?.Description,
                Size = file?.Size,
                CreatedAt = file?.CreatedAt?.DateTime,
                LastModifiedAt = file?.LastModifiedAt?.DateTime,
                DownloadUrl = file?.DownloadUrl
            };
            return model;
        }
    }
}
