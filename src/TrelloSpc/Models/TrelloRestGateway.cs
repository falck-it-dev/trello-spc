using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrelloSpc.Models
{
    /// <summary>
    /// Constructs a URI for quering for trello data, queries trello, and returns the json result.
    /// </summary>
    public interface ITrelloRestGateway
    {
        string GetCardsForBoard(string boardId);
        string GetCardWithHistory(string cardId);
    }

    public class TrelloRestGateway : ITrelloRestGateway
    {
        public string GetCardsForBoard(string boardId)
        {
            throw new NotImplementedException();
        }

        public string GetCardWithHistory(string cardId)
        {
            throw new NotImplementedException();
        }
    }
}