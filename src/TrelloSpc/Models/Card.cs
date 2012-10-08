namespace TrelloSpc.Models
{
    public class Card
    {
        public string Id { get; set; }

        public string Name
        {
            get { return TrelloName; }
        }

        /// <summary>
        /// The name of the card in Trello. Contains both the estimate and the human readable name
        /// </summary>
        public string TrelloName { get; set; }
    }
}