using System.Collections.Generic;
using GraphSdkDemo.SharePoint.Contracts.Enums;

namespace GraphSdkDemo.SharePoint.Contracts.Files
{
    public class DocumentLibrariesResult
    {
        public ICollection<DocumentLibrary> DocumentLibraries { get; private set; }

        public ConnectionStatus Status { get; private set; }

        public string ErrorMessage { get; private set; }

        public static DocumentLibrariesResult Success(ICollection<DocumentLibrary> documentLibraries)
        {
            return new DocumentLibrariesResult
            {
                Status = ConnectionStatus.Success,
                DocumentLibraries = documentLibraries
            };
        }

        public static DocumentLibrariesResult Failed(string errorMessage)
        {
            return new DocumentLibrariesResult
            {
                Status = ConnectionStatus.Error,
                ErrorMessage = errorMessage
            };
        }
    }
}
