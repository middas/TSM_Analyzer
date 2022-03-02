using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSM.Logic.Extensions
{
    public static class DictionaryExtensions
    {
        public static T MergeLeft<T,K,V>(this T me, params IDictionary<K,V>[] others) where T : IDictionary<K,V>, new()
        {
            T newMap = new();
            foreach(IDictionary<K,V> src in (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                foreach(KeyValuePair<K,V> pair in src)
                {
                    newMap[pair.Key] = pair.Value;
                }
            }

            return newMap;
        }
    }
}
