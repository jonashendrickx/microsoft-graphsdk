using System.Net.Http;
using System.Threading.Tasks;

namespace GraphSdkDemo.Net.HttpRequests
{
    public interface IOAuthManager
    {
        /// <summary>
        ///     String indexer to get or set OAuth parameter values.
        /// </summary>
        /// <param name="index">The OAuth parameter without the oauth_ prefix.</param>
        string this[string index] { get; set; }

        /// <summary>
        ///     Acquires a request token from the given uri, using the given HTTP method.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="method">The HTTP method used.</param>
        /// <returns>A response object that contains the entire text of the response as well as the extracted parameters.</returns>
        Task<OAuthResponse> AcquireRequestToken(string uri, HttpMethod method);

        /// <summary>
        ///     Acquires an access token from the given uri, using the given Http method.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="method">The HTTP method used.</param>
        /// <param name="token">The </param>
        /// <param name="verifier">The previously acquired request verifier.</param>
        /// <returns></returns>
        Task<OAuthResponse> AcquireAccessToken(string uri, HttpMethod method, string token, string verifier);

        /// <summary>
        ///     Generates a string to be used in the authorization header in a HTTP request.
        /// </summary>
        /// <param name="uri">The uri.</param>
        /// <param name="method">The HTTP method used.</param>
        /// <returns>A string to be used in the authorization header.</returns>
        string GenerateAuthenticationHeader(string uri, HttpMethod method);
    }
}