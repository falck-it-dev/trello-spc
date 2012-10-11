using System;

namespace TrelloSpc.Models
{
    /// <summary>
    /// Represents that a card has been in a specific list for a period of time.
    /// </summary>
    public class ListHistoryItem
    {
        public List List { get; set; }

        /// <summary>
        /// Gets the time when the card was transferred to the list. It this is unknown, then the value is <c>null</c>.
        /// </summary>
        public DateTime? StartTimeUtc { get; set; }
        /// <summary>
        /// Gets the time when the card was transferred to the list. It the card is still in the list, then the value is <c>null</c>.
        /// </summary>
        public DateTime? EndTimeUtc { get; set; }

        public string ListName
        {
            get { return List == null ? null : List.Name; }
        }

        public TimeSpan Time
        {
            get { return ((EndTimeUtc ?? DateTime.UtcNow) - StartTimeUtc) ?? TimeSpan.Zero; }
        }
    }
}