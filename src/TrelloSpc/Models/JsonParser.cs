﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace TrelloSpc.Models
{
    public interface IJsonParser
    {
        IEnumerable<Card> GetCards(string jsonResponse);
        void ProcessCardHistory(Card card, string cardJson, ListLookupFunction getList);
        IEnumerable<List> GetLists(string jsonResponse);
    }

    public class Action
    {
        public const string CreateCard = "createCard";
        public const string UpdateCard = "updateCard";
        public const string MoveCardToBoard = "moveCardToBoard";
        public const string ConvertToCardFromCheckItem = "convertToCardFromCheckItem";
    }

    public class JsonParser : IJsonParser
    {        
        public IEnumerable<Card> GetCards(string jsonResponse)
        {
            var jObject = JObject.Parse(jsonResponse);
            var cards = GetCards(jObject);

            return cards.Values;
        }

        public IEnumerable<List> GetLists(string jsonResponse)
        {
            var jObject = JObject.Parse(jsonResponse);
            var lists = (JArray)jObject["lists"];
            if (lists == null)
                return null;
            return lists.Select(CreateList);
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

        public void ProcessCardHistory(Card card, string cardJson, ListLookupFunction getList)
        {
            var jObject = JObject.Parse(cardJson);
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
                        var list = getList(listId);
                        card.ListHistory.Add(new ListHistoryItem
                        {
                            List = list,
                            StartTimeUtc = time
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
                            var sourceList = getList(listBefore["id"].Value<string>());
                            var destList = getList(listAfter["id"].Value<string>());
                            if (card.ListHistory.Count == 0)
                                card.ListHistory.Add(new ListHistoryItem());
                            var listItem = card.ListHistory.Last();
                            listItem.EndTimeUtc = time;
                            if (listItem.List == null)
                                listItem.List = sourceList;
                            else
                                if (listItem.List != sourceList) throw new ApplicationException("UpdateCard parse error. listBefore does not match previous list.");
                            card.ListHistory.Add(new ListHistoryItem
                            {
                                List = destList,
                                StartTimeUtc = time
                            });
                        }
                        break;
                    case Action.MoveCardToBoard:
                        card.ListHistory.Last().EndTimeUtc = time;
                        card.ListHistory.Add(new ListHistoryItem
                        {
                            List = null, // Unknown at present
                            StartTimeUtc = time
                        });
                        break;
                    case Action.ConvertToCardFromCheckItem :
                        card.ListHistory.Add(new ListHistoryItem { StartTimeUtc = time });
                        break;
                }
            }
            var lastItem = card.ListHistory.Last();
            lastItem.List = lastItem.List ?? card.List;
        }
    }
}