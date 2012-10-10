using NUnit.Framework;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.Model.CardTests
{
    [TestFixture]
    public class SetTrelloNameTest
    {
        [Test]
        public void ShouldInitializeNameWhenNoPointsGiven()
        {
            var card = new Card { TrelloName = "Name" };
            Assert.That(card.Name, Is.EqualTo("Name"));
        }

        [Test]
        public void ShouldStripPointsFromNameWhenPointsGiven()
        {
            var card = new Card { TrelloName = "(5) Name" };
            Assert.That(card.Name, Is.EqualTo("Name"));
        }

        [Test]
        public void ShouldSetPointsToNullWhenNoPointsGiven()
        {
            var card = new Card { TrelloName = "Name" };
            Assert.That(card.Points, Is.Null);
        }

        [Test]
        public void ShouldSetPointsWhenPointsGiven()
        {
            var card = new Card { TrelloName = "(5) Name" };
            Assert.That(card.Points, Is.EqualTo(5));
        }

        [Test]
        public void ShouldResetNameAndPointsWhenAssignedToNull()
        {
            var card = new Card { TrelloName = "(5) Name" };
            card.TrelloName = null;
            Assert.That(card.Name, Is.Null);
            Assert.That(card.Points, Is.Null);
        }
    }
}
