using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using GraphSdkDemo.Core.Providers.Contracts;

namespace GraphSdkDemo.Net.Handlers
{
    public class CertificateWebRequestHandler : HttpClientHandler
    {
        private ICertificateProvider _certificateProvider;
        private readonly string _certificate;
        private readonly string _password;
        private bool _disposed;

        public CertificateWebRequestHandler()
        {
            
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            ClientCertificates.Add(new X509Certificate());
            //var certificate = _certificateProvider.Load(_certificate, _password);
            //var requestHandler = new WebRequestHandler();
            //requestHandler.ClientCertificates.Add(certificate);
            return await base.SendAsync(request, cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
