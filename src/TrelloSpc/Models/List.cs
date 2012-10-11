using System;
using System.Collections.Generic;
using System.Linq;

namespace TrelloSpc.Models
{
    public class List
    {
        public string Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Creates a dictionary of a number of lists, using the <see cref="List.Id"/> as the key.
        /// </summary>
        public static Dictionary<string, List> CreateDictionary(params List[] lists)
        {
            return lists.ToDictionary(x => x.Id);
        }

        public override string ToString()
        {
            return String.Format("List id: {0}, Name: {1}", Id, Name);
        }
    }
}