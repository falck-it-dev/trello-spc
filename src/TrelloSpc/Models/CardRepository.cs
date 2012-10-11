using System;
using System.Collections.Generic;
using System.Linq;

namespace TrelloSpc.Models
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetCardsForBoard(string boardId);
        //IEnumerable<Card> NewGetCardsForBoard(string boardId);
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
            var cardsJson = _trelloRestGateway.GetCardsForBoard(boardId);
            var cards = _jsonParser.GetCards(cardsJson).ToArray();
            var lists = _jsonParser.GetLists(cardsJson).ToArray();
            var listLookupFunction = List.CreateLookupFunction(lists);
            foreach (var card in cards)
            {
                var cardJson = _trelloRestGateway.GetCardWithHistory(card.Id);
                _jsonParser.ProcessCardHistory(card, cardJson, listLookupFunction);
            }
            return cards;
        }
    }
}