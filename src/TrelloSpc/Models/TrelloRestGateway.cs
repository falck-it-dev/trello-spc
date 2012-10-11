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
        private readonly ITrelloGateway _trelloGateway;
        private readonly ITrelloConfiguration _trelloConfiguration;

        public TrelloRestGateway(
            ITrelloGateway trelloGateway, 
            ITrelloConfiguration trelloConfiguration)
        {
            _trelloGateway = trelloGateway;
            _trelloConfiguration = trelloConfiguration;
        }

        public string GetCardsForBoard(string boardId)
        {
            var url = string.Format("https://api.trello.com/1/boards/{0}?cards=all&lists=all&key={1}&token={2}",
                boardId,
                _trelloConfiguration.AppKey,
                _trelloConfiguration.UserToken);

            return _trelloGateway.GetJsonData(url);
        }

        public string GetCardWithHistory(string cardId)
        {
            var actions = String.Join(",",
                new[] {
                    Action.ConvertToCardFromCheckItem,
                    Action.CreateCard,
                    Action.MoveCardToBoard,
                    Action.UpdateCard });

            var url = string.Format("https://api.trello.com/1/cards/{0}?actions={1},updateCard,moveCardToBoard&actions_limit=1000&key={2}&token={3}",
                cardId,
                actions,
                _trelloConfiguration.AppKey,
                _trelloConfiguration.UserToken);
            return _trelloGateway.GetJsonData(url);
        }
    }
}