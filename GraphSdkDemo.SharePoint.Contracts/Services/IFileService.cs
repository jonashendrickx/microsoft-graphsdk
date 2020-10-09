using System.Threading.Tasks;
using GraphSdkDemo.SharePoint.Contracts.Files;

namespace GraphSdkDemo.SharePoint.Contracts.Services
{
    public interface IFileService
    {
        Task<DocumentLibrariesResult> GetDocumentLibrariesAsync(string siteId);
        Task<FilesResult> GetFilesAsync(string siteId, string documentLibraryId);
        Task<DownloadResult> DownloadAsync(string siteId, string documentLibraryId, string driveItemId);
    }
}
