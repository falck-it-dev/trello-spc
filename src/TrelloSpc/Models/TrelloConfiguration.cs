using System.Configuration;

namespace TrelloSpc.Models
{
    public interface ITrelloConfiguration
    {
        string DefaultBoardId { get; }
    }

    public class TrelloConfiguration : ITrelloConfiguration
    {
        public string DefaultBoardId
        {
            get { return ConfigurationManager.AppSettings["TrelloBoardId"]; }
        }
    }
}