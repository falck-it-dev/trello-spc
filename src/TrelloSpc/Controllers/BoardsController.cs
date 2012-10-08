using System;
using System.Linq;
using System.Web.Mvc;
using TrelloSpc.Models;

namespace TrelloSpc.Controllers
{
    public class BoardsController : Controller
    {
        private readonly ITrelloConfiguration _trelloConfiguration;
        private readonly ICardRepository _cardRepository;

        public BoardsController(
            ITrelloConfiguration trelloConfiguration, 
            ICardRepository cardRepository)
        {
            _trelloConfiguration = trelloConfiguration;
            _cardRepository = cardRepository;
        }

        //
        // GET: /Boards/
        [HttpGet]
        public ActionResult Index()
        {
            var defaultBoardId = _trelloConfiguration.DefaultBoardId;
            return RedirectToAction("Get", new { Id = defaultBoardId });
        }

        [HttpGet]
        public ActionResult Get(string id)
        {
            var cards = _cardRepository.GetCardsForBoard(id);
            var viewModel = new BoardViewModel
            {
                Cards = cards.ToArray()
            };
            return View("Board", viewModel);            
        }
    }
}
