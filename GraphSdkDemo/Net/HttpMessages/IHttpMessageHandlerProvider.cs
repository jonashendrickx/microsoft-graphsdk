using System.Net.Http;

namespace GraphSdkDemo.Net.HttpMessages
{
    public interface IHttpMessageHandlerProvider
    {
        HttpMessageHandler Get(HttpMessageHandlerOption option);
    }
}