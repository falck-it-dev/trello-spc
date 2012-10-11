using System;
using System.Collections.Generic;
using System.Linq;

namespace TrelloSpc.Models
{
    public delegate List ListLookupFunction(string id);

    public class List
    {
        public string Id { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Creates a dictionary of a number of lists, using the <see cref="List.Id"/> as the key.
        /// </summary>
        public static ListLookupFunction CreateLookupFunction(params List[] lists)
        {
            var dict = lists.ToDictionary(x => x.Id);
            return id =>
                {
                    List result;
                    if (dict.TryGetValue(id, out result)) return result;
                    result = new List { Id = id };
                    dict.Add(id, result);
                    return result;
                };
            
        }

        public override string ToString()
        {
            return String.Format("List id: {0}, Name: {1}", Id, Name);
        }
    }
}