using System;
using System.Collections.Generic;
using System.Linq;

namespace TrelloSpc.Models
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCardsForBoard(string boardId);
        IEnumerable<Card> NewGetCardsForBoard(string boardId);
    }

    public class CardRepository : ICardRepository
    {
        private readonly ITrelloConfiguration _trelloConfiguration;
        private readonly ITrelloGateway _trelloGateway;
        private readonly ITrelloRestGateway _trelloRestGateway;
        private readonly IJsonParser _jsonParser;

        public CardRepository(
            ITrelloConfiguration trelloConfiguration, 
            ITrelloGateway trelloGateway, 
            IJsonParser jsonParser, 
            ITrelloRestGateway trelloRestGateway)
        {
            _trelloConfiguration = trelloConfiguration;
            _trelloGateway = trelloGateway;
            _jsonParser = jsonParser;
            _trelloRestGateway = trelloRestGateway;
        }

        public IEnumerable<Card> GetCardsForBoard(string boardId)
        {
            var url = string.Format("https://api.trello.com/1/boards/{0}?cards=all&lists=all&actions=updateCard&actions_limit=1000&key={1}&token={2}",
                boardId,
                _trelloConfiguration.AppKey,
                _trelloConfiguration.UserToken);

            var jsonData = _trelloGateway.GetJsonData(url);
            return _jsonParser.GetCards(jsonData);
        }

        public IEnumerable<Card> NewGetCardsForBoard(string boardId)
        {
            var cardsJson = _trelloRestGateway.GetCardsForBoard(boardId);
            var cards = _jsonParser.GetCards(cardsJson).ToArray();
            var listLookupFunction = List.CreateLookupFunction();
            foreach (var card in cards)
            {
                var cardJson = _trelloRestGateway.GetCardWithHistory(card.Id);
                _jsonParser.ProcessCardHistory(card, cardJson, listLookupFunction);
            }
            return cards;
        }
    }
}