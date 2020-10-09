using System.IO;
using System.Net;
using System.Net.Http.Headers;

namespace GraphSdkDemo.Net.HttpRequests
{
    public class HttpDownloadResult
    {
        public HttpRequestHeaders HttpRequestHeaders { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public Stream Stream { get; set; }

        public CookieCollection CookieCollection { get; set; }

        public HttpResponseHeaders HttpResponseHeaders { get; set; }
    }
}