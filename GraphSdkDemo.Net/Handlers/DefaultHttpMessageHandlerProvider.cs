using System.Net.Http;
using GraphSdkDemo.Net.HttpMessages;

namespace GraphSdkDemo.Net.Handlers
{
    public class DefaultHttpMessageHandlerProvider : IHttpMessageHandlerProvider
    {
        public HttpMessageHandler Get(HttpMessageHandlerOption option)
        {
            if (option.Certificate == null)
            {
                return new HttpClientHandler
                {
                    CookieContainer = option.CookieContainer,

                };
            }

            return new HttpClientHandler
            {
                ClientCertificates = { option.Certificate },
                CookieContainer = option.CookieContainer
            };
        }
    }
}