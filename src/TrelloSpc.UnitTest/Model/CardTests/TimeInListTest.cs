using System;
using NUnit.Framework;
using TrelloSpc.Models;
using List = TrelloSpc.Models.List;

namespace TrelloSpc.UnitTest.Model.CardTests
{
    [TestFixture]
    public class TimeInListTest
    {
        [Test]
        public void ShouldReturnZeroWhenUnknownList()
        {
            var card = new Card();
            var actual = card.TimeInList("dummy");
            Assert.That(actual, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void ShouldReturnTimeInListWhenCardInListOnce()
        {
            // Setup
            var time1 = DateTime.UtcNow;
            var time2 = time1.AddMinutes(1);
            var list1 = new List { Name = "LIST1" };
            var list2 = new List { Name = "LIST2" };
            var card = new Card();
            card.MoveToList(list1, time1);
            card.MoveToList(list2, time2);

            // Exercise
            var actual = card.TimeInList("LIST1");

            // Verify
            var expected = TimeSpan.FromMinutes(1);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
