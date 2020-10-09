using GraphSdkDemo.SharePoint.Contracts.Files;
using System.Runtime.Serialization;

namespace GraphSdkDemo.SharePoint.Api.Contracts.File
{
    [DataContract]
    public class DocumentLibraryModelInfo
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        public static DocumentLibraryModelInfo FromView(DocumentLibrary view)
        {
            return new DocumentLibraryModelInfo
            {
                Id = view?.Id,
                Name = view?.Name
            };
        }
    }
}
