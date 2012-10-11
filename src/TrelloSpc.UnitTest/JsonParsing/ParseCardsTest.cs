using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.JsonParsing
{
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
    }
}
