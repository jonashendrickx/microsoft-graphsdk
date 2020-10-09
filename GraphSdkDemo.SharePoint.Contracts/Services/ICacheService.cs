using System.Threading.Tasks;

namespace GraphSdkDemo.SharePoint.Contracts.Services
{
    public interface ICacheService
    {
        Task DownloadAsync();
        Task<bool> IsExistsAsync(string filename);
        Task UploadAsync();
    }
}
