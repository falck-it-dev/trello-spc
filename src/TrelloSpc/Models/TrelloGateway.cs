using System.IO;
using System.Net;
using System.Web;

namespace TrelloSpc.Models
{
    public interface ITrelloGateway
    {
        string GetJsonData(string url);
    }

    /// <summary>
    /// Gateway for sending URL requests to Trello. Wraps functionality
    /// for opening an HTTP request, and configuring an HTTP proxy.
    /// </summary>
    public class TrelloGateway : ITrelloGateway
    {
        private readonly INetworkConfiguration _networkConfiguration;

        public TrelloGateway(INetworkConfiguration networkConfiguration)
        {
            _networkConfiguration = networkConfiguration;
        }

        public string GetJsonData(string uri)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(uri);
            if (_networkConfiguration.HttpProxy != null)
                webRequest.Proxy = new WebProxy(_networkConfiguration.HttpProxy);
            var proxy = webRequest.Proxy;
            if (proxy != null && _networkConfiguration.UseNtlmProxyAuthentication)
                proxy.Credentials = CredentialCache.DefaultCredentials;

            using (var response = webRequest.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}