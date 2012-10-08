using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.JsonParsing
{
    [TestFixture]
    public class ParseCardsTest
    {
        [Test]
        public void ShouldReturnCardsWithNameInitialized()
        {
            // Setup
            var parser = new JsonParser();
            var json = @"{""id"":""board-id"",""name"":""Board name"",
                          ""cards"":[
                              {""id"":""id-1"",""name"":""Card 1""},
                              {""id"":""id-2"",""name"":""Card 2""}
                        ]}";
            

            // Exercise
            var cards = parser.GetCards(json);

            // Verify
            var expected = new[] {
                new { Id = "id-1", TrelloName = "Card 1" },
                new { Id = "id-2", TrelloName = "Card 2" }};
            var actual = cards.Select(x => new { x.Id, x.TrelloName }).ToArray();
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
