using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ForumSiteCore.Utility
{
    public static class CollectionExtensions
    {
        public static Dictionary<T1, T2> MergeDictionaries<T1, T2>(List<Dictionary<T1,T2>> dictionaries)
        {           
            // order the dictionaries we're merging by count.
            // the more entries we have in one of them, the less we have to add from the others.
            List<Dictionary<T1, T2>> orderedByCount = dictionaries.OrderByDescending(x => x.Count).ToList();
            var finalDictionary = dictionaries.First();

            for(int x = 1; x < dictionaries.Count; x++)
            {
                var curDictionary = dictionaries[x];
                foreach (var item in curDictionary)
                {
                    finalDictionary.TryAdd(item.Key, item.Value);
                }
            }

            return finalDictionary;
        }
    }
}
