using System;
using System.Web.Mvc;
using TrelloSpc.Models;

namespace TrelloSpc.Controllers
{
    public class BoardsController : Controller
    {
        private readonly ITrelloConfiguration _trelloConfiguration;

        public BoardsController(ITrelloConfiguration trelloConfiguration)
        {
            _trelloConfiguration = trelloConfiguration;
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
            throw new NotImplementedException();
        }
    }
}
