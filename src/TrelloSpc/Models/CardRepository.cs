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
        public IEnumerable<Card> GetCardsForBoard(string boardId)
        {
            throw new NotImplementedException();
        }
    }
}