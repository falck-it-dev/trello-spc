using System.Configuration;

namespace TrelloSpc.Models
{
    public interface ITrelloConfiguration
    {
        string DefaultBoardId { get; }
        string AppKey { get; }
        string UserToken { get; }
    }

    public class TrelloConfiguration : ITrelloConfiguration
    {
        public string DefaultBoardId
        {
            get { return ConfigurationManager.AppSettings["TrelloBoardId"]; }
        }

        public string AppKey
        {
            get { return ConfigurationManager.AppSettings["trelloAppKey"]; }
        }

        public string UserToken
        {
            get { return ConfigurationManager.AppSettings["trelloUserToken"]; }
        }
    }
}