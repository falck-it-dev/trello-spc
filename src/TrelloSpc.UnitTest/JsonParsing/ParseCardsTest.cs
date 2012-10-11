using System;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using TrelloSpc.Models;
using TrelloSpc.UnitTest.Helpers;
using Action = TrelloSpc.Models.Action;
using List = TrelloSpc.Models.List;

namespace TrelloSpc.UnitTest.JsonParsing
{
    [TestFixture]
    public class ParseCardsTest
    {
        private JsonParser _parser;

        private static object GetActualHistory(Card card)
        {
            var actualListHistory = card.ListHistory.Select(x =>
                                                            new
                                                            {
                                                                x.List,
                                                                StartTime = x.StartTimeUtc,
                                                                EndTime = x.EndTimeUtc
                                                            }).ToArray();
            return actualListHistory;
        }

        [SetUp]
        public void Setup()
        {
            _parser = new JsonParser();
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
            
            // Exercise
            var cards = _parser.GetCards(data.ToJson());

            // Verify
            var expected = new[] {
                new { Id = "id-1", TrelloName = "Card 1" },
                new { Id = "id-2", TrelloName = "Card 2" }};
            var actual = cards.Select(x => new { x.Id, x.TrelloName }).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void ShouldNotComplainAboutUnknownCardOrListId()
        {
            var data = new { actions = new [] { new { data = new { card = new { id = "bad-card-id" } } } } ,
                             cards = new object[0] };
            var json = data.ToJson();
            Assert.That(() => _parser.GetCards(json), Throws.Nothing);
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
            var lists = List.CreateLookupFunction(list1, list2);
            var card = new Card { Id = "card-id", List = list2 };
            var actions = new { actions = new[] { 
                // Keep order, Trello sends newest items first in list
                moveCardAction,
                createCardAction  } 
            };

            // Exercise
            _parser.ProcessCardHistory(card, actions.ToJson(), lists);

            // verify
            var actualListHistory = GetActualHistory(card);
            var expectedListHistory = new[] {
                new { List = list1, StartTime = (DateTime?)createTime, EndTime = (DateTime?)moveToBoardTime },
                new { List = list2, StartTime = (DateTime?)moveToBoardTime, EndTime = (DateTime?)null }};
            Assert.That(actualListHistory, Is.EqualTo(expectedListHistory));
        }

        [Test]
        public void ShouldTraceHistoryForToBoardAndThenMoved()
        {
            // Setup
            var list1 = new List { Id = "list-1-id" };
            var list2 = new List { Id = "list-2-id" };
            var list3 = new List { Id = "list-3-id" };
            var lists = List.CreateLookupFunction(list1, list2, list3);
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
            _parser.ProcessCardHistory(card, actions.ToJson(), lists);

            // verify
            var actualListHistory = GetActualHistory(card);
            var expectedListHistory = new[] {
                new { List = list1, StartTime = (DateTime?)time1, EndTime = (DateTime?)time2 },
                new { List = list2, StartTime = (DateTime?)time2, EndTime = (DateTime?)time3 },
                new { List = list3, StartTime = (DateTime?)time3, EndTime = (DateTime?)null }};
            Assert.That(actualListHistory, Is.EqualTo(expectedListHistory));
        }

        [Test]
        public void ShouldHandlerConvertCardFromCheckList()
        {
            // Setup
            var time1 = DateTime.Parse("2012-01-01");
            var list1 = new List {Id = "list-1-id"};
            var card = new Card {Id = "card-id", List = list1};
            var lists = List.CreateLookupFunction(list1);
            var convertCardAction = new
                {
                    type = Action.ConvertToCardFromCheckItem,
                    date = time1.ToString("o")
                };

            var actions = new
                {
                    actions = new[]
                        {
                            convertCardAction
                        }
                };

            // Exercise
            _parser.ProcessCardHistory(card, actions.ToJson(), lists);

            // Verify
            var actualListHistory = GetActualHistory(card);
            var expectedListHistory = new[]
                {
                    new {List = list1, StartTime = (DateTime?)time1, EndTime = (DateTime?) null},
                };
            Assert.That(actualListHistory, Is.EqualTo(expectedListHistory));
        }

        [Test]
        public void ShouldNotThrowExceptionWhenStartHistoryIsMissing()
        {
            // Setup
            var time = DateTime.Parse("2012-1-1");
            var actions = new
            {
                actions = new[] {
                JsonObjectHelpers.MoveToListAction(time, "card-id", "list-1-id", "list-2-id") }
            };
            var list1 = new List { Id = "list-1-id" };
            var list2 = new List { Id = "list-2-id" };
            var lists = List.CreateLookupFunction(list1, list2);
            var card = new Card { Id = "card-id", List = list1 };
            // Exercise
            _parser.ProcessCardHistory(card, actions.ToJson(), lists);

            // Verify
            var actualListHistory = GetActualHistory(card);
            var expectedListHistory = new[]
                {
                    new {List = list1, StartTime = (DateTime?)null, EndTime = (DateTime?)time },
                    new {List = list2, StartTime = (DateTime?)time, EndTime = (DateTime?) null},
                };
            Assert.That(actualListHistory, Is.EqualTo(expectedListHistory));
        }
    }
}
