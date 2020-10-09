using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GraphSdkDemo.Core.Providers.Contracts;
using GraphSdkDemo.Net.Handlers;
using GraphSdkDemo.Net.HttpMessages;
using GraphSdkDemo.Net.HttpRequests;

namespace GraphSdkDemo.Net
{
    public class HttpRequest : IHttpRequest
    {
        private readonly CookieContainer _cookieContainer = new CookieContainer();
        private readonly List<Func<DelegatingHandler>> _delegatingHandlers = new List<Func<DelegatingHandler>>();
        private readonly IHttpMessageHandlerProvider _handlerProvider;
        private readonly ICertificateProvider _certificateProvider;
        private readonly IDictionary<string, string> _messageHeaders = new Dictionary<string, string>();
        private X509Certificate2 _certificate;

        public HttpRequest(IHttpMessageHandlerProvider handlerProvider)
        {
            if (handlerProvider == null) throw new ArgumentNullException(nameof(handlerProvider));
            _handlerProvider = handlerProvider;
        }

        public HttpRequest(IHttpMessageHandlerProvider handlerProvider, ICertificateProvider certificateProvider)
        {
            if (handlerProvider == null) throw new ArgumentNullException(nameof(handlerProvider));
            if (certificateProvider == null) throw new ArgumentNullException(nameof(certificateProvider));
            _handlerProvider = handlerProvider;
            _certificateProvider = certificateProvider;
        }

        /// <summary>
        ///     Adds an http message handler for the request.
        /// </summary>
        /// <param name="handler"></param>
        public IHttpRequest AddHandler(Func<DelegatingHandler> handler)
        {
            _delegatingHandlers.Add(handler);
            return this;
        }

        /// <summary>
        ///     Sets an additional header for the request.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        public IHttpRequest SetHeader(string name, string value)
        {
            if (_messageHeaders.ContainsKey(name)) _messageHeaders.Remove(name);
            _messageHeaders.Add(name, value);
            return this;
        }

        /// <summary>
        ///     Adds a certificate to the request.
        /// </summary>
        public void AddCertificate(string key, string password)
        {
            _certificate = _certificateProvider.Load(key, password);
        }

        /// <summary>
        ///     Sets a basic authentication header for the request.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public IHttpRequest AddBasicAuthentication(string username, string password)
        {
            _delegatingHandlers.Add(() => new BasicHttpMessageHandler(username, password));
            return this;
        }

        /// <summary>
        ///     Sets a bearer authentication header for the request.
        /// </summary>
        /// <param name="bearerToken">The bearer token.</param>
        public IHttpRequest AddBearerAuthentication(string bearerToken)
        {
            _delegatingHandlers.Add(() => new BearerTokenHttpMessageHandler(bearerToken));
            return this;
        }

        /// <summary>
        ///     Adds a cookie to the cookie collection for the request.
        /// </summary>
        /// <param name="cookie">The cookie to be sent along with the request.</param>
        public IHttpRequest AddCookie(Cookie cookie)
        {
            _cookieContainer.Add(cookie);
            return this;
        }

        /// <summary>
        ///     Loads a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="httpMethod">The HTTP method to use.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpRequestResult> Load(Uri requestUri, HttpMethod httpMethod)
        {
            return InternalLoad(requestUri, httpMethod);
        }

        /// <summary>
        ///     Downloads a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpDownloadResult> DownloadResponse(Uri requestUri)
        {
            return InternalDownload(requestUri);
        }


        /// <summary>
        ///     Gets a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpRequestResult> GetResponse(Uri requestUri)
        {
            return InternalLoad(requestUri, HttpMethod.Get);
        }

        /// <summary>
        ///     Delete a resource from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpRequestResult> Delete(Uri requestUri)
        {
            return InternalLoad(requestUri, HttpMethod.Delete);
        }

        /// <summary>
        ///     Gets a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="data">The data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpRequestResult> PostData(Uri requestUri, Dictionary<string, string> data)
        {
            return InternalPostData(requestUri, new FormUrlEncodedContent(data));
        }

        /// <summary>
        ///     Gets a response from a POST call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="content">The content data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpRequestResult> PostData(Uri requestUri, HttpContent content)
        {
            return InternalPostData(requestUri, content);
        }

        /// <summary>
        ///     Gets a response from a POST call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="json">The JSON data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpRequestResult> PostData(Uri requestUri, string json)
        {
            return InternalPostData(requestUri, HttpMethod.Post, json);
        }

        /// <summary>
        ///     Gets a response from a PUT call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="json">The JSON data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        public Task<HttpRequestResult> PutData(Uri requestUri, string json)
        {
            return InternalPostData(requestUri, HttpMethod.Put, json);
        }

        #region Private methods

        private static HttpMessageHandler CreatePipeline(HttpMessageHandler innerHandler, IEnumerable<Func<DelegatingHandler>> handlers)
        {
            if (handlers == null)
            {
                return innerHandler;
            }

            // Wire handlers up in reverse order starting with the inner handler
            var pipeline = innerHandler;
            var reversedHandlers = handlers.Reverse();
            foreach (var handler in reversedHandlers)
            {
                var delegatingHandler = handler();
                delegatingHandler.InnerHandler = pipeline;
                pipeline = delegatingHandler;
            }

            return pipeline;
        }

        private async Task<HttpRequestResult> InternalLoad(Uri requestUri, HttpMethod httpMethod)
        {
            try
            {
                using (var httpMessageHandler = _handlerProvider.Get(new HttpMessageHandlerOption(_cookieContainer, _certificate)))
                {
                    using (var httpClient = new HttpClient(CreatePipeline(httpMessageHandler, _delegatingHandlers)))
                    {
                        using (var request = new HttpRequestMessage(httpMethod, requestUri))
                        {
                            foreach (var header in _messageHeaders)
                            {
                                request.Headers.Add(header.Key, header.Value);
                            }

                            var response = await httpClient.SendAsync(request);
                            var content = await response.Content.ReadAsStringAsync();

                            return new HttpRequestResult
                            {
                                HttpRequestHeaders = request.Headers,
                                HttpStatusCode = response.StatusCode,
                                Content = content,
                                CookieCollection = _cookieContainer.GetCookies(requestUri),
                                HttpResponseHeaders = response.Headers
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                return new HttpRequestResult
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Content = exception.Message,
                    CookieCollection = _cookieContainer.GetCookies(requestUri)
                };
            }
        }

        private async Task<HttpDownloadResult> InternalDownload(Uri requestUri)
        {
            try
            {
                using (var httpMessageHandler = _handlerProvider.Get(new HttpMessageHandlerOption(_cookieContainer, _certificate)))
                {
                    using (var httpClient = new HttpClient(CreatePipeline(httpMessageHandler, _delegatingHandlers)))
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                        {
                            foreach (var header in _messageHeaders)
                            {
                                request.Headers.Add(header.Key, header.Value);
                            }

                            var response = await httpClient.SendAsync(request);
                            var stream = await response.Content.ReadAsStreamAsync();

                            return new HttpDownloadResult
                            {
                                HttpRequestHeaders = request.Headers,
                                HttpStatusCode = response.StatusCode,
                                Stream = stream,
                                CookieCollection = _cookieContainer.GetCookies(requestUri),
                                HttpResponseHeaders = response.Headers
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                return new HttpDownloadResult
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    ErrorMessage = exception.Message,
                    CookieCollection = _cookieContainer.GetCookies(requestUri)
                };
            }
        }

        private async Task<HttpRequestResult> InternalPostData(Uri requestUri, HttpContent requestContent)
        {
            try
            {
                using (var httpMessageHandler = _handlerProvider.Get(new HttpMessageHandlerOption(_cookieContainer, _certificate)))
                {
                    using (var httpClient = new HttpClient(CreatePipeline(httpMessageHandler, _delegatingHandlers)))
                    {
                        using (var request = new HttpRequestMessage(HttpMethod.Post, requestUri))
                        {
                            foreach (var header in _messageHeaders)
                            {
                                request.Headers.Add(header.Key, header.Value);
                            }
                            request.Content = requestContent;

                            var response = await httpClient.SendAsync(request);
                            var responseContent = await response.Content.ReadAsStringAsync();

                            return new HttpRequestResult
                            {
                                HttpRequestHeaders = request.Headers,
                                HttpContentHeaders = request.Content.Headers,
                                HttpStatusCode = response.StatusCode,
                                Content = responseContent,
                                CookieCollection = _cookieContainer.GetCookies(requestUri),
                                HttpResponseHeaders = response.Headers
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                return new HttpRequestResult
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Content = exception.InnerException?.Message,
                    CookieCollection = _cookieContainer.GetCookies(requestUri)
                };
            }
        }

        private async Task<HttpRequestResult> InternalPostData(Uri requestUri, HttpMethod httpMethod, string json)
        {
            try
            {
                using (var httpMessageHandler = _handlerProvider.Get(new HttpMessageHandlerOption(_cookieContainer, _certificate)))
                {
                    using (var httpClient = new HttpClient(CreatePipeline(httpMessageHandler, _delegatingHandlers)))
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        using (var request = new HttpRequestMessage(httpMethod, requestUri))
                        {
                            foreach (var header in _messageHeaders)
                            {
                                request.Headers.Add(header.Key, header.Value);
                            }
                            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                            var response = await httpClient.SendAsync(request);
                            var content = await response.Content.ReadAsStringAsync();

                            return new HttpRequestResult
                            {
                                HttpRequestHeaders = request.Headers,
                                HttpStatusCode = response.StatusCode,
                                Content = content,
                                CookieCollection = _cookieContainer.GetCookies(requestUri),
                                HttpResponseHeaders = response.Headers
                            };
                        }
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                return new HttpRequestResult
                {
                    HttpStatusCode = HttpStatusCode.NotFound,
                    Content = exception.InnerException?.Message,
                    CookieCollection = _cookieContainer.GetCookies(requestUri)
                };
            }
        }

        #endregion
    }
}