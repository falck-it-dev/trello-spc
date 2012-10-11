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

    public class Action
    {
        public const string CreateCard = "createCard";
        public const string UpdateCard = "updateCard";
        public const string MoveCardToBoard = "moveCardToBoard";
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
                Card card;
                if (!cards.TryGetValue(cardId, out card)) continue;
                var listBefore = data["listBefore"];
                var listAfter = data["listAfter"];                
                if (listBefore != null && listAfter != null)
                {
                    var listId = listAfter["id"];                    
                    var list = lists[listId.Value<string>()];
                    var time = jAction["date"].Value<DateTime>();
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

        public static void ProcessCardHistory(Card card, JObject jObject, Dictionary<string, List> lists)
        {
            var actions = (JArray)jObject["actions"];
            if (actions == null)
                throw new ApplicationException("No actions found");

            var actionsOrdered = actions.OrderBy(x => x["date"].Value<DateTime>());
            foreach (var action in actionsOrdered)
            {
                var cardId = (string)action["id"];
                var type = action["type"].Value<string>();
                var time = action["date"].Value<DateTime>();
                switch (type)
                {
                    case Action.CreateCard :
                        var listId = action["data"]["list"]["id"].Value<string>();
                        var list = lists[listId];
                        card.ListHistory.Add(new ListHistoryItem
                        {
                            List = list,
                            StartTime = time
                        });
                        break;
                    case Action.UpdateCard:
                        var data = action["data"];
                        var listBefore = data["listBefore"];
                        var listAfter = data["listAfter"];
                        if (listBefore != null || listAfter != null)
                        {
                            if (listBefore == null) throw new ApplicationException("UpdateCard parse error. listAfter specified, but listBefore not specified. Card: " + cardId);
                            if (listAfter == null) throw new ApplicationException("UpdateCard parse error. listBefore specified, but listAfter not specified. Card: " + cardId);
                            var sourceList = lists[listBefore["id"].Value<string>()];
                            var destList = lists[listAfter["id"].Value<string>()];
                            var listItem = card.ListHistory.Last();
                            listItem.EndTime = time;
                            if (listItem.List == null)
                                listItem.List = sourceList;
                            else
                                if (listItem.List != sourceList) throw new ApplicationException("UpdateCard parse error. listBefore does not match previous list.");
                            card.ListHistory.Add(new ListHistoryItem
                            {
                                List = destList,
                                StartTime = time
                            });
                        }
                        break;
                    case Action.MoveCardToBoard:
                        card.ListHistory.Last().EndTime = time;
                        card.ListHistory.Add(new ListHistoryItem
                        {
                            List = null, // Unknown at present
                            StartTime = time
                        });
                        break;
                }
            }
            var lastItem = card.ListHistory.Last();
            lastItem.List = lastItem.List ?? card.List;
        }
    }
}