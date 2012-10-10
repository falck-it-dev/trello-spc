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
            var jObject = JObject.Parse(jsonResponse);
            var lists = GetLists(jObject);
            var cards = GetCards(jObject);

            ProcessActions(jObject, lists, cards);

            return cards.Values;
        }

        private void ProcessActions(JToken jObject, Dictionary<string, List> lists, Dictionary<string, Card> cards)
        {
            var jActions = jObject["actions"];
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
                    var card = cards[cardId];
                    var list = lists[listId.Value<string>()];
                    card.MoveToList(list, time);
                }
            }
        }

        private Dictionary<string, List> GetLists(JObject jObject)
        {            
            var lists = (JArray)jObject["lists"];
            if (lists == null)
                return null;
            return lists.Select(CreateList).ToDictionary(x => x.Id);
        }

        private Dictionary<string, Card> GetCards(JObject jObject)
        {
            var cards = (JArray)jObject["cards"];            
            if (cards == null)
                throw new ApplicationException("Cannot read cards from json response.\r\nJson response:\r\n" + jObject);
            return cards.Select(CreateCard).ToDictionary(x => x.Id);
        }

        private static Card CreateCard(JToken jToken)
        {
            return new Card
                {
                    Id = (string) jToken["id"],
                    TrelloName = (string) jToken["name"]
                };
        }

        private static List CreateList(JToken jToken)
        {
            var id = (string) jToken["id"];
            var name = (string) jToken["name"];
            var list = new List {Id = id, Name = name};
            return list;
        }
    }
}