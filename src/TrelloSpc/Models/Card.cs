using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TrelloSpc.Models
{
    public class Card
    {
        private string _trelloName;        
        private readonly List<ListHistoryItem> _listHistory = new List<ListHistoryItem>();

        public string Id { get; set; }

        public string Name { get; private set; }

        public int? Points { get; private set; }

        /// <summary>
        /// The name of the card in Trello. Contains both the estimate and the human readable name
        /// </summary>
        public string TrelloName
        {
            get { return _trelloName; }
            set
            {
                _trelloName = value;
                if (value == null)
                {
                    Name = null;
                    Points = null;
                }
                else
                {
                    var match = Regex.Match(value, @"(\((?<points>\d+)\))?(?<name>.*)");
                    var group1 = match.Groups["points"];
                    if (group1.Success)
                        Points = int.Parse(group1.Value);
                    Name = match.Groups["name"].Value.Trim();
                }
            }
        }

        public List<ListHistoryItem> ListHistory
        {
            get { return _listHistory; }
        }

        public List List { get; set; }

        public TimeSpan TimeInList(string listName)
        {
            var result = TimeSpan.Zero;
            foreach (var item in ListHistory.Where(x => x.ListName == listName))
                result += item.Time;
            return result;
        }
    }
}