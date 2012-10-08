using Moq;
using NUnit.Framework;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.Repositories
{
    [TestFixture]
    public class CardRepositoryTest : AutoMockHelper
    {
        [Test]
        public void ShouldUseCardDeserializerToReturnCards()
        {
            // Setup
            var repository = GetInstance<CardRepository>();
            var trelloGatewayMock = GetMock<ITrelloGateway>();
            var parserMock = GetMock<IJsonParser>();
            var cards = new[] { new Card(), new Card() };
            trelloGatewayMock
                .Setup(x => x.GetJsonData(It.IsAny<string>()))
                .Returns("JSON-RESPONSE");
            parserMock
                .Setup(x => x.GetCards("JSON-RESPONSE"))
                .Returns(cards);

            // Exercise
            var result = repository.GetCardsForBoard("BOARD-ID");

            // Verify
            Assert.That(result, Is.EqualTo(cards));
        }

        [Test]
        public void ShouldUseCorrectUrlForLoadingCards()
        {
            // Setup
            var repository = GetInstance<CardRepository>();
            var trelloConfigurationMock = GetMock<ITrelloConfiguration>();
            trelloConfigurationMock.Setup(x => x.AppKey).Returns("APP-KEY");
            trelloConfigurationMock.Setup(x => x.UserToken).Returns("TOKEN");

            // Exercise
            repository.GetCardsForBoard("BOARD-ID");

            // Verify
            var trelloGatewayMock = GetMock<ITrelloGateway>();
            const string expectedUrl = "https://api.trello.com/1/boards/BOARD-ID?cards=open&key=APP-KEY&token=TOKEN";
            trelloGatewayMock.Verify(x => x.GetJsonData(expectedUrl));
        }
    }
}
