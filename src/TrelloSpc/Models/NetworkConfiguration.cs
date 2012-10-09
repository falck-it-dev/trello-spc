using System.Configuration;

namespace TrelloSpc.Models
{
    public interface INetworkConfiguration
    {
        bool UseNtlmProxyAuthentication { get; }
        string HttpProxy { get; }
    }

    public class NetworkConfiguration : INetworkConfiguration
    {
        public bool UseNtlmProxyAuthentication
        {
            get { return ConfigurationManager.AppSettings["useNtlmProxyAuthentication"].ToLower().Trim() == "true"; }
        }

        public string HttpProxy
        {
            get { return ConfigurationManager.AppSettings["httpProxy"]; }
        }
    }
}