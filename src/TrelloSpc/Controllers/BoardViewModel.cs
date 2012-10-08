using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrelloSpc.Models;

namespace TrelloSpc.Controllers
{
    /// <summary>
    /// Viewmodel for Views/Boards/Board
    /// </summary>
    public class BoardViewModel
    {
        public Card[] Cards { get; set; }
    }
}