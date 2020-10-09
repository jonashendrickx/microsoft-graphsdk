using System.Collections.Generic;
using System.Linq;

namespace GraphSdkDemo.Net.HttpRequests
{
    public class OAuthResponse
    {
        private readonly Dictionary<string, string> _parameters;

        public OAuthResponse(string response)
        {
            Response = response;
            _parameters = new Dictionary<string, string>();
            var kvpairs = response.Split('&');
            foreach (var kv in kvpairs.Select(pair => pair.Split('=')))
            {
                _parameters.Add(kv[0], kv[1]);
            }
        }

        public string Response { get; set; }

        public string this[string index] => _parameters[index];
    }
}