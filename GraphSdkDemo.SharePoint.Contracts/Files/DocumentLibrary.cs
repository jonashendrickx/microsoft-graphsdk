using System;
using Microsoft.Graph;

namespace GraphSdkDemo.SharePoint.Contracts.Files
{
    public class DocumentLibrary
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public static DocumentLibrary FromView(Drive view)
        {
            if (view == null) throw new ArgumentNullException(nameof(view));

            var result = new DocumentLibrary
            {
                Id = view.Id,
                Name = view.Name
            };

            return result;
        }
    }
}
