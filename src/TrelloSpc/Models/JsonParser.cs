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
            var lists = GetLists(jObject);
            var jCards = jObject["cards"];
            if (jCards == null)
                throw new ApplicationException("Cannot read cards from json response.\r\nJson response:\r\n" + jsonResponse);

            var cards = ((JArray)jCards).Select(CreateCard);
            var cardsDict = cards.ToDictionary(x => x.Id);

            var jActions = jObject["actions"];
            ProcessActions(jActions, lists, cardsDict);

            return cardsDict.Values;
        }

        private void ProcessActions(JToken jActions, Dictionary<string, List> lists, Dictionary<string, Card> cardsDict)
        {
            if (jActions == null)
                return;
            foreach (var jAction in (JArray)jActions)
            {
                var data = jAction["data"];
                var cardId = (string)data["card"]["id"];
                var listBefore = data["listBefore"];
                var listAfter = data["listAfter"];
                var time = jAction["date"].Value<DateTime>();
                if (listBefore != null && listAfter != null)
                {
                    var listId = listAfter["id"];
                    var card = cardsDict[cardId];
                    var list = lists[listId.Value<string>()];
                    card.MoveToList(list, time);
                }
            }
        }

        private static Card CreateCard(JToken jToken)
        {
            return new Card
                {
                    Id = (string) jToken["id"],
                    TrelloName = (string) jToken["name"]
                };
        }

        private Dictionary<string, List> GetLists(JObject jObject)
        {
            var result = new Dictionary<string, List>();
            var lists = jObject["lists"];
            
            if (lists != null)
                foreach (var list in lists)
                {
                    var id = (string)list["id"];
                    var name = (string)list["name"];
                    result[id] = new List { Id = id, Name = name };
                }
            return result;
        }
    }
}