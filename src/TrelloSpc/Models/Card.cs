using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TrelloSpc.Models
{
    public class MoveToListAction : IComparable<MoveToListAction>
    {
        public List List { get; set; }
        public DateTime UtcTime { get; set; }

        public int CompareTo(MoveToListAction other)
        {
            return UtcTime.CompareTo(other.UtcTime);
        }
    }

    public class Card
    {
        private string _trelloName;
        private readonly List<MoveToListAction> _actions = new List<MoveToListAction>();

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

        public List<MoveToListAction> Actions
        {
            get { return _actions; }
        }

        public TimeSpan TimeInList(string listName)
        {
            var result = TimeSpan.Zero;
            DateTime? timeIntoList = null;
            Actions.Sort();
            foreach (var action in Actions)
            {
                if (action.List.Name == listName)
                {
                    timeIntoList = timeIntoList ?? action.UtcTime;
                }
                else
                {
                    if (timeIntoList != null)
                    {
                        result += action.UtcTime - timeIntoList.Value;
                        timeIntoList = null;
                    }
                }
            }
            if (timeIntoList != null)
                result += DateTime.UtcNow - timeIntoList.Value;
            return result;
        }

        public void MoveToList(List list, DateTime utcTimeMovedToList)
        {
            Actions.Add(new MoveToListAction
            {
                List = list,
                UtcTime = utcTimeMovedToList
            });
        }
    }
}