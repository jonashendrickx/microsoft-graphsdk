using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace GraphSdkDemo.Net.HttpMessages
{
    public class HttpMessageHandlerOption
    {
        public HttpMessageHandlerOption(CookieContainer cookieContainer, X509Certificate2 certificate)
        {
            CookieContainer = cookieContainer;
            Certificate = certificate;
        }

        public CookieContainer CookieContainer { get; }
        public X509Certificate2 Certificate { get; }
    }
}