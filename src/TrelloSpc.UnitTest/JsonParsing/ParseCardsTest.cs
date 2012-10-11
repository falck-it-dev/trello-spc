using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using TrelloSpc.Models;
using List = TrelloSpc.Models.List;

namespace TrelloSpc.UnitTest.JsonParsing
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            return JObject.FromObject(obj).ToString();
        }
    }

    [TestFixture]
    public class ParseCardsTest
    {
        private string ToJson(object obj)
        {
            return JObject.FromObject(obj).ToString();
        }

        private IEnumerable<Card> ParseCardsJson(string json)
        {
            var parser = new JsonParser();
            return parser.GetCards(json);
        }

        [Test]
        public void ShouldReturnCardsWithNameInitialized()
        {
            // Setup
            var data = new
            {
                id = "board-id",
                name = "Board name",
                cards = new[] {
                    new { id = "id-1", name = "Card 1" },
                    new { id = "id-2", name = "Card 2" } }
            };
            var json = ToJson(data); 

            // Exercise
            var cards = ParseCardsJson(json);

            // Verify
            var expected = new[] {
                new { Id = "id-1", TrelloName = "Card 1" },
                new { Id = "id-2", TrelloName = "Card 2" }};
            var actual = cards.Select(x => new { x.Id, x.TrelloName }).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldInitializeListChanges()
        {
            // Setup
            var data = new {
                actions = new[] {
                    new { data = new { 
                            card = new { id = "card-id" }, 
                            listBefore = new { id = "list-1-id" }, 
                            listAfter = new { id = "list-2-id" }},
                          date = "2012-10-10T12:01:02Z" }},
                cards = new[] { 
                    new { id = "card-id" } },
                lists = new[] {
                    new { id = "list-1-id", name = "list-1" },
                    new { id = "list-2-id", name = "list-2" }}
            };
            var json = ToJson(data);

            // Exercise
            var cards = ParseCardsJson(json);
                    
            // Verify
            var card = cards.Single();
            var actual = card.Actions.Select(x => new 
                { 
                    ListName = x.List.Name, 
                    x.UtcTime 
                }).ToArray();
            var expected = new[] {
                new { 
                    ListName = "list-2", 
                    UtcTime = new DateTime(2012,10,10,12,01,02)
                }};
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldNotComplainAboutUnknownCardOrListId()
        {
            var data = new { actions = new [] { new { data = new { card = new { id = "bad-card-id" } } } } ,
                             cards = new object[0] };
            var json = ToJson(data);
            Assert.That(() => ParseCardsJson(json), Throws.Nothing);
        }

        [Test]
        public void ShouldTraceHistoryForCardMovedToNewBoard()
        {
            // Setup
            var list1 = new List { Id = "list-1-id" };
            var list2 = new List { Id = "list-2-id" };
            var createTime = DateTime.Parse("2012-01-01");
            var moveToBoardTime = createTime.AddMinutes(5);

            var createCardAction = JsonObjectHelpers.CreateCardAction(createTime, "card-id", "list-1-id");
            var moveCardAction = JsonObjectHelpers.MoveCardAction(moveToBoardTime, "card-id");
            var lists = List.CreateDictionary(list1, list2);
            var card = new Card { Id = "card-id", List = list2 };
            var actions = new { actions = new[] { 
                // Keep order, Trello sends newest items first in list
                moveCardAction,
                createCardAction  } 
            };

            // Exercise
            JsonParser.ProcessCardHistory(card, JObject.FromObject(actions), lists);

            // verify
            var actualListHistory = card.ListHistory.Select(x =>
                new
                {
                    x.List,
                    x.StartTime,
                    x.EndTime
                }).ToArray();
            var expectedListHistory = new[] {
                new { List = list1, StartTime = createTime, EndTime = (DateTime?)moveToBoardTime },
                new { List = list2, StartTime = moveToBoardTime, EndTime = (DateTime?)null }};
            Assert.That(actualListHistory, Is.EqualTo(expectedListHistory));
        }

        [Test]
        public void ShouldTraceHistoryForToBoardAndThenMoved()
        {
            // Setup
            var list1 = new List { Id = "list-1-id" };
            var list2 = new List { Id = "list-2-id" };
            var list3 = new List { Id = "list-3-id" };
            var lists = List.CreateDictionary(list1, list2, list3);
            var time1 = DateTime.Parse("2012-01-01 12:00");
            var time2 = time1.AddHours(1);
            var time3 = time2.AddHours(2);
            
            var actions = new
            {
                actions = new object[] {
                    // Keep order, Trello sends newest items first in list
                    JsonObjectHelpers.MoveToListAction(time3, "card-id", "list-2-id", "list-3-id"),
                    JsonObjectHelpers.MoveCardAction(time2, "card-id") ,
                    JsonObjectHelpers.CreateCardAction(time1, "card-id", "list-1-id")
                }
            };
            var card = new Card { Id = "card-id", List = list3 };
            
            // Exercise
            JsonParser.ProcessCardHistory(card, JObject.FromObject(actions), lists);

            // verify
            var actualListHistory = card.ListHistory.Select(x =>
                new
                {
                    x.List,
                    x.StartTime,
                    x.EndTime
                }).ToArray();
            var expectedListHistory = new[] {
                new { List = list1, StartTime = time1, EndTime = (DateTime?)time2 },
                new { List = list2, StartTime = time2, EndTime = (DateTime?)time3 },
                new { List = list3, StartTime = time3, EndTime = (DateTime?)null }};
            Assert.That(actualListHistory, Is.EqualTo(expectedListHistory));
        }
    }
}
