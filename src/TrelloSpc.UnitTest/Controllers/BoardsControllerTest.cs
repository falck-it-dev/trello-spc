using System.Web.Mvc;
using NUnit.Framework;
using TrelloSpc.Controllers;
using TrelloSpc.Models;

namespace TrelloSpc.UnitTest.Controllers
{
    [TestFixture]
    public class BoardsControllerTest : AutoMockHelper
    {
        [Test]
        public void IndexShouldRedirectToDefaultBoard()
        {
            // Setup            
            var controller = GetInstance<BoardsController>();
            var configurationMock = GetMock<ITrelloConfiguration>();
            configurationMock.Setup(x => x.DefaultBoardId).Returns("BOARD-ID");

            // Exercise
            var actionResult = controller.Index();

            // Verify
            var redirectResult = (RedirectToRouteResult)actionResult;
            Assert.That(redirectResult.RouteValues["controller"], Is.Null);
            Assert.That(redirectResult.RouteValues["action"], Is.EqualTo("Get"));
            Assert.That(redirectResult.RouteValues["id"], Is.EqualTo("BOARD-ID"));
        }
    }
}
