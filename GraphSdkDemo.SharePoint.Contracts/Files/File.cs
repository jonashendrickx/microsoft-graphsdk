using Microsoft.Graph;
using System;

namespace GraphSdkDemo.SharePoint.Contracts.Files
{
    public class File
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? Size { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public string DownloadUrl { get; set; }

        public static File FromView(DriveItem view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            var result = new File
            {
                Id = view.Id,
                Name = view.Name,
                CreatedAt = view.CreatedDateTime,
                LastModifiedAt = view.LastModifiedDateTime,
                Description = view.Description,
                Size = view.Size,
                DownloadUrl = view.AdditionalData?["@microsoft.graph.downloadUrl"]?.ToString()
            };

            return result;
        }
    }
}
