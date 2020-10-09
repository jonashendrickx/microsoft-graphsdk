using System.Net.Http;
using GraphSdkDemo.Net.HttpMessages;

namespace GraphSdkDemo.Net.Handlers
{
    public class TestHttpMessageHandlerProvider : IHttpMessageHandlerProvider
    {
        private readonly HttpMessageHandler _messageHandler;

        public TestHttpMessageHandlerProvider(HttpMessageHandler messageHandler)
        {
            _messageHandler = messageHandler;
        }

        public HttpMessageHandler Get(HttpMessageHandlerOption option)
        {
            return _messageHandler;
        }
    }
}