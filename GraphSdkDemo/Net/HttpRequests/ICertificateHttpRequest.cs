using System;
using System.Threading.Tasks;

namespace GraphSdkDemo.Net.HttpRequests
{
    public interface ICertificateHttpRequest
    {
        /// <summary>
        ///     Adds a certificate to the request.
        /// </summary>
        void AddCertificate(string key, string password);

        /// <summary>
        ///     Sets a bearer authentication header for the request.
        /// </summary>
        /// <param name="bearerToken">The bearer token.</param>
        ICertificateHttpRequest AddBearerAuthentication(string bearerToken);

        /// <summary>
        ///     Gets a response from a POST call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="json">The JSON data to POST.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> PostData(Uri requestUri, string json);


        /// <summary>
        ///     Gets a response from a POST call to an external uri.
        /// </summary>
        /// <param name="requestUri">The external uri.</param>
        /// <param name="json">The JSON data to POST.</param>
        /// <param name="referenceId">The reference id.</param>
        /// <param name="type">The type of HTTP request.</param>
        /// <param name="provider">The provider reference.</param>
        /// <returns>The response received from the external uri.</returns>
        Task<HttpRequestResult> PostData(Uri requestUri, string json, Guid referenceId, HttpRequestType type, string provider);
    }
}
