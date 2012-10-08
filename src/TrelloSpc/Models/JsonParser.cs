using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrelloSpc.Models
{
    public interface IJsonParser
    {
        IEnumerable<Card> GetCards(string jsonResponse);
    }

    public class JsonParser : IJsonParser
    {
        public IEnumerable<Card> GetCards(string jsonResponse)
        {
            throw new NotImplementedException();
        }
    }
}