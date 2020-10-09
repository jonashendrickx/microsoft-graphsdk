using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using GraphSdkDemo.SharePoint.Contracts.Files;
using GraphSdkDemo.SharePoint.Contracts.Services;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;

namespace GraphSdkDemo.SharePoint.Services
{
    public class FileService : IFileService
    {
        private readonly GraphServiceClient _graphServiceClient;
        private readonly string _username;
        private readonly SecureString _password;

        public FileService(ISettingsProvider settings)
        {
            var clientId = settings.GetClientId();
            var tenantId = settings.GetTenantId();
            IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .Build();

            var scopes = new List<string>();
            scopes.Add("User.Read");
            scopes.Add("AllSites.Read");
            scopes.Add("MyFiles.Read");

            UsernamePasswordProvider authProvider = new UsernamePasswordProvider(publicClientApplication, scopes);

            _graphServiceClient = new GraphServiceClient(authProvider);

            _username = settings.GetServiceAccountUsername();

            _password = new SecureString();
            var password = settings.GetServiceAccountPassword();
            password.ToCharArray().ToList().ForEach(x => _password.AppendChar(x));
        }

        public async Task<DocumentLibrariesResult> GetDocumentLibrariesAsync(string siteId)
        {
            try
            {
                var drives = await _graphServiceClient.Sites[siteId].Drives.Request().WithUsernamePassword(_username, _password).GetAsync();
                return DocumentLibrariesResult.Success(drives.AsEnumerable().Select(DocumentLibrary.FromView).ToList());
            }
            catch (ServiceException e)
            {
                return DocumentLibrariesResult.Failed(e.InnerException?.Message ?? e.Error?.Message ?? e.Message);
            }

        }

        public async Task<FilesResult> GetFilesAsync(string siteId, string documentLibraryId)
        {
            try
            {
                var drives = await _graphServiceClient.Sites[siteId].Drives[documentLibraryId].Root.Children.Request()
                    .WithUsernamePassword(_username, _password).GetAsync();
                return FilesResult.Success(drives.AsEnumerable().Select(Contracts.Files.File.FromView).ToList());

            }
            catch (ServiceException e)
            {
                return FilesResult.Failed(e.InnerException?.Message ?? e.Error?.Message ?? e.Message);

            }
        }

        public async Task<DownloadResult> DownloadAsync(string siteId, string documentLibraryId, string driveItemId)
        {
            try
            {
                var driveItem = await _graphServiceClient.Sites[siteId].Drives[documentLibraryId].Items[driveItemId].Request().WithUsernamePassword(_username, _password).GetAsync();
                
                var downloadedItem = await _graphServiceClient.Sites[siteId].Drives[documentLibraryId].Items[driveItemId].Content.Request().GetAsync();

                var inputBuffer = new byte[downloadedItem.Length];
                downloadedItem.Read(inputBuffer, 0, inputBuffer.Length);

                return DownloadResult.Success(driveItemId, driveItem.Name, inputBuffer);
            }
            catch (ServiceException e)
            {
                return DownloadResult.Failed(e.InnerException?.Message ?? e.Error?.Message ?? e.Message);
            }
        }
    }
}
