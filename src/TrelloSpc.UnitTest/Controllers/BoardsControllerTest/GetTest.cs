using System.Web.Mvc;
using NUnit.Framework;
using TrelloSpc.Controllers;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.Controllers.BoardsControllerTest
{
    [TestFixture]
    public class GetTest : AutoMockHelper
    {
        [Test]
        public void ShouldRetrieveCardsAndStoreInViewInfo()
        {
            // Setup
            var cards = new[] { new Card(), new Card() };
            var repositoryMock = GetMock<ICardRepository>();
            repositoryMock
                .Setup(x => x.GetCardsForBoard("BOARD-ID"))
                .Returns(cards);
            var controller = GetInstance<BoardsController>();

            // Exercise
            var result = controller.Get("BOARD-ID");

            // Verify
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewData["Cards"], Is.EqualTo(cards));
        }
    }
}
