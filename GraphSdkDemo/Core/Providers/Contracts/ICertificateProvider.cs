using System.Security.Cryptography.X509Certificates;

namespace GraphSdkDemo.Core.Providers.Contracts
{
    public interface ICertificateProvider
    {
        X509Certificate Load(string key);
        X509Certificate2 Load(string key, string password);
        X509Certificate2 Load(string key, string password, X509KeyStorageFlags flags);
    }
}