using System;
using Action = TrelloSpc.Models.Action;

namespace TrelloSpc.UnitTest.JsonParsing
{
    public static class JsonObjectHelpers
    {
        public static object MoveCardAction(DateTime moveToBoardTime, string cardId)
        {
            var moveCardAction = new
                {
                    type = "moveCardToBoard",
                    date = moveToBoardTime.ToString("o"),
                    card = new { id = cardId }
                };
            return moveCardAction;
        }

        public static object CreateCardAction(DateTime createTime, string cardId, string listId)
        {
            var createCardAction = new
                {
                    type = Action.CreateCard,
                    date = createTime.ToString("o"),
                    data = new
                        {
                            card = new { id = cardId },
                            list = new { id = listId }
                        }
                };
            return createCardAction;
        }

        public static object MoveToListAction(DateTime time3, string cardId, string sourceListId, string destListId)
        {
            var moveToListAction = new
                {
                    type = "updateCard",
                    date = time3.ToString("o"),
                    card = new {id = cardId},
                    data = new
                        {
                            listBefore = new {id = sourceListId},
                            listAfter = new {id = destListId}
                        }
                };
            return moveToListAction;
        }
    }
}