using GraphSdkDemo.SharePoint.Contracts.Enums;

namespace GraphSdkDemo.SharePoint.Contracts.Files
{
    public class DownloadResult
    {
        public string Id { get; private set; }

        public string Name { get; private set; }

        public byte[] Content { get; private set; }

        public ConnectionStatus Status { get; private set; }

        public string ErrorMessage { get; private set; }

        public static DownloadResult Success(string id, string name, byte[] content)
        {
            return new DownloadResult
            {
                Status = ConnectionStatus.Success,
                Id = id,
                Name = name,
                Content = content
            };
        }

        public static DownloadResult Failed(string errorMessage)
        {
            return new DownloadResult
            {
                Status = ConnectionStatus.Error,
                ErrorMessage = errorMessage
            };
        }
    }
}
