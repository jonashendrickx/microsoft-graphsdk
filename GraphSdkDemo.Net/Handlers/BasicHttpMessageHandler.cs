using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GraphSdkDemo.Net.Handlers
{
    public class BasicHttpMessageHandler : DelegatingHandler
    {
        private readonly string _password;
        private readonly string _username;
        private bool _disposed;

        public BasicHttpMessageHandler(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public BasicHttpMessageHandler(string username, string password, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _username = username;
            _password = password;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_username}:{_password}"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
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