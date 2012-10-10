using Moq;
using NUnit.Framework;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.Repositories
{
    [TestFixture]
    public class CardRepositoryTest : AutoMockHelper
    {
        private Mock<ITrelloConfiguration> _trelloConfigurationMock;
        private CardRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = GetInstance<CardRepository>();
            _trelloConfigurationMock = GetMock<ITrelloConfiguration>();
        }

        private void UrlShouldMatch(string pattern)
        {
            var trelloGatewayMock = GetMock<ITrelloGateway>();
            trelloGatewayMock.Verify(x => x.GetJsonData(It.IsRegex(pattern)));
        }

        [Test]
        public void ShouldUseCardDeserializerToReturnCards()
        {
            // Setup
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
            var result = _repository.GetCardsForBoard("BOARD-ID");

            // Verify
            Assert.That(result, Is.EqualTo(cards));
        }

        [Test]
        public void ShouldUseCorrectUrlForLoadingCards()
        {
            // Setup
            _trelloConfigurationMock.Setup(x => x.AppKey).Returns("APP-KEY");
            _trelloConfigurationMock.Setup(x => x.UserToken).Returns("TOKEN");

            // Exercise
            _repository.GetCardsForBoard("BOARD-ID");

            // Verify            
            UrlShouldMatch("https://api.trello.com/1/boards/BOARD-ID?.*&key=APP-KEY&token=TOKEN");           
        }

        [Test]
        public void ShouldQueryForCards()
        {
            _repository.GetCardsForBoard("DUMMY");
            UrlShouldMatch("cards=all");
        }

        [Test]
        public void ShouldQueryForLists()
        {
            _repository.GetCardsForBoard("DUMMY");
            UrlShouldMatch("lists=all");
        }

        [Test]
        public void ShouldQueryForActions()
        {
            _repository.GetCardsForBoard("DUMMY");
            UrlShouldMatch("actions=updateCard");
        }
    }
}
