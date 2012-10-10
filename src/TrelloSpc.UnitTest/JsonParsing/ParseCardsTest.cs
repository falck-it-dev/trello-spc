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

        [Test]
        public void ShouldInitializeListChanges()
        {
            // Setup
            var parser = new JsonParser();
            var json = @"{""actions"":[
                                {""data"":
                                    {""card"":{""id"":""card-id""},
                                     ""listAfter"":{""id"":""list-2-id""},
                                     ""listBefore"":{""id"":""list-1-id""}},
                                 ""date"":""2012-10-10T12:01:02Z""}
                             ],
                          ""cards"":[
                                {""id"":""card-id""}
                            ],
                          ""lists"":[
                                {""id"":""list-1-id"",""name"":""list-1""},
                                {""id"":""list-2-id"",""name"":""list-2""}]}";

            // Exercise
            var cards = parser.GetCards(json);
                    
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
    }
}
