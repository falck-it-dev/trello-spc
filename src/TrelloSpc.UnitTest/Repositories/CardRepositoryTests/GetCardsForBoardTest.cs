using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.Repositories.CardRepositoryTests
{
    [TestFixture]
    public class GetCardsForBoardTest : AutoMockHelper
    {
        [Test]
        public void ShouldWork()
        {
            // Setup
            var gatewayMock = GetMock<ITrelloRestGateway>();
            var parserMock = GetMock<IJsonParser>();
            var repository = GetInstance<CardRepository>();
            var card = new Card { Id = "card-id" };

            gatewayMock
                .Setup(x => x.GetCardsForBoard("BOARD-ID"))
                .Returns("cards-json");
            gatewayMock
                .Setup(x => x.GetCardWithHistory("card-id"))
                .Returns("card-history-json");
            parserMock
                .Setup(x => x.GetCards("cards-json"))
                .Returns(new [] { card});
            
            // Exercise
            var cards = repository.NewGetCardsForBoard("BOARD-ID");
            
            // Verify
            Assert.That(cards.ToArray(), Is.EqualTo(new[] { card }));
            parserMock.Verify(x => x.ProcessCardHistory(card, "card-history-json", It.IsAny<ListLookupFunction>()));
        }
    }
}
