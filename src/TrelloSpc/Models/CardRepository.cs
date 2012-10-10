using System;
using System.Collections.Generic;

namespace TrelloSpc.Models
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCardsForBoard(string boardId);
    }

    public class CardRepository : ICardRepository
    {
        private readonly ITrelloConfiguration _trelloConfiguration;
        private readonly ITrelloGateway _trelloGateway;
        private readonly IJsonParser _jsonParser;

        public CardRepository(
            ITrelloConfiguration trelloConfiguration, 
            ITrelloGateway trelloGateway, 
            IJsonParser jsonParser)
        {
            _trelloConfiguration = trelloConfiguration;
            _trelloGateway = trelloGateway;
            _jsonParser = jsonParser;
        }

        public IEnumerable<Card> GetCardsForBoard(string boardId)
        {
            var url = string.Format("https://api.trello.com/1/boards/{0}?cards=all&lists=all&actions=updateCard&key={1}&token={2}",
                boardId,
                _trelloConfiguration.AppKey,
                _trelloConfiguration.UserToken);

            var jsonData = _trelloGateway.GetJsonData(url);
            return _jsonParser.GetCards(jsonData);
        }
    }
}