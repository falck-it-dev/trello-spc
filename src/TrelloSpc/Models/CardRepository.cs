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
            yield return new Card { Name = "Card1" };
            yield return new Card { Name = "Card2" };
            yield return new Card { Name = "Card3" };
        }
    }
}