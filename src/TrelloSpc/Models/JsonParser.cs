using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

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
            JObject jObject = JObject.Parse(jsonResponse);
            var jCards = jObject["cards"];
            if (jCards == null)
                throw new ApplicationException("Cannot read cards from json response.\r\nJson response:\r\n" + jsonResponse);

            return from jCard in (JArray)jCards
                   select new Card
                   {
                       Id = (string)jCard["id"],
                       TrelloName = (string)jCard["name"]
                   };
        }
    }
}