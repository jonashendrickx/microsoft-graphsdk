using System.Collections.Generic;
using GraphSdkDemo.SharePoint.Contracts.Enums;

namespace GraphSdkDemo.SharePoint.Contracts.Files
{
    public class FilesResult
    {
        public ICollection<File> Files { get; private set; }

        public ConnectionStatus Status { get; private set; }

        public string ErrorMessage { get; private set; }

        public static FilesResult Success(ICollection<File> files)
        {
            return new FilesResult
            {
                Status = ConnectionStatus.Success,
                Files = files
            };
        }

        public static FilesResult Failed(string errorMessage)
        {
            return new FilesResult
            {
                Status = ConnectionStatus.Error,
                ErrorMessage = errorMessage
            };
        }
    }
}
