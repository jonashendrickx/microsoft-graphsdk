using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GraphSdkDemo.Net.HttpRequests
{
    /// <summary>
    ///     Http message handling that supports different type of authentication.
    /// </summary>
    public interface IHttpRequest
    {
        /// <summary>
        ///     Adds a cookie to the cookie collection for the request.
        /// </summary>
        /// <param name="cookie">The cookie to be sent along with the request.</param>
        IHttpRequest AddCookie(Cookie cookie);

        /// <summary>
        ///     Adds an http message handler for the request.
        /// </summary>
        /// <param name="handler">The handler.</param>
        IHttpRequest AddHandler(Func<DelegatingHandler> handler);

        /// <summary>
        ///     Sets an additional header for the request.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        /// <param name="value">The value of the header.</param>
        IHttpRequest SetHeader(string name, string value);

        /// <summary>
        ///     Adds a certificate to the request.
        /// </summary>
        void AddCertificate(string key, string password);

        /// <summary>
        ///     Sets a basic authentication header for the request.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        IHttpRequest AddBasicAuthentication(string username, string password);

        /// <summary>
        ///     Sets a bearer authentication header for the request.
        /// </summary>
        /// <param name="bearerToken">The bearer token.</param>
        IHttpRequest AddBearerAuthentication(string bearerToken);

        /// <summary>
        ///     Loads a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="httpMethod">The HTTP method to use.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> Load(Uri requestUri, HttpMethod httpMethod);

        /// <summary>
        ///     Downloads a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpDownloadResult> DownloadResponse(Uri requestUri);

        /// <summary>
        ///     Gets a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> GetResponse(Uri requestUri);

        /// <summary>
        ///     Gets a response from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="data">The data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> PostData(Uri requestUri, Dictionary<string, string> data);

        /// <summary>
        ///     Gets a response from a POST call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="json">The JSON data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> PostData(Uri requestUri, string json);

        /// <summary>
        ///     Gets a response from a PUT call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="json">The JSON data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> PutData(Uri requestUri, string json);

        /// <summary>
        ///     Gets a response from a POST call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="content">The content data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> PostData(Uri requestUri, HttpContent content);

        /// <summary>
        ///     Delete a resource from an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> Delete(Uri requestUri);
    }
}