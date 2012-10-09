using NUnit.Framework;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.Model
{
    [TestFixture]
    public class CardTest
    {
        [Test]
        public void NameShouldReturnTrelloNameWhenNoStoryPointGiven()
        {
            var card = new Card { TrelloName = "Name" };
            Assert.That(card.Name, Is.EqualTo("Name"));
        }

        [Test]
        public void NameShouldReturnStrippedTrelloNameWhenPointGiven()
        {
            var card = new Card { TrelloName = "(5) Name" };
            Assert.That(card.Name, Is.EqualTo("Name"));
        }

        [Test]
        public void PointShouldBeNullWhenNoPointsGiven()
        {
            var card = new Card { TrelloName = "Name" };
            Assert.That(card.Points, Is.Null);
        }

        [Test]
        public void PointShouldEqualCardPointsWhenGiven()
        {
            var card = new Card { TrelloName = "(5) Name" };
            Assert.That(card.Points, Is.EqualTo(5));
        }

        [Test]
        public void ResettingTrelloNameShouldResetPointAndName()
        {
            var card = new Card { TrelloName = "(5) Name" };
            card.TrelloName = null;
            Assert.That(card.Name, Is.Null);
            Assert.That(card.Points, Is.Null);
        }
    }
}
