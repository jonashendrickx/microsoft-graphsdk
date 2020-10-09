using System.Net;
using System.Net.Http.Headers;

namespace GraphSdkDemo.Net.HttpRequests
{
    public class HttpRequestResult
    {
        public HttpRequestHeaders HttpRequestHeaders { get; set; }
        public HttpResponseHeaders HttpResponseHeaders { get; set; }
        public HttpContentHeaders HttpContentHeaders { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Content { get; set; }
        public CookieCollection CookieCollection { get; set; }
    }
}