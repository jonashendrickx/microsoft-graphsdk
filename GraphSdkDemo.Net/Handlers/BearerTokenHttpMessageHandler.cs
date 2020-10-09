using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSdkDemo.Net.Handlers
{
    public class BearerTokenHttpMessageHandler : DelegatingHandler
    {
        private readonly string _bearerToken;
        private bool _disposed;

        public BearerTokenHttpMessageHandler(string bearerToken)
        {
            _bearerToken = bearerToken;
        }

        public BearerTokenHttpMessageHandler(string bearerToken, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _bearerToken = bearerToken;
        }


        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
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