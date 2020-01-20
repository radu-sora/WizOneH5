using System.Collections.Generic;

namespace Wizrom.Reports.Code
{
    public static class DictionaryExtensions
    {
        public static List<int> GetValue(this Dictionary<string, List<int>> dictionary, string key)
        {
            List<int> value = null;

            if (!dictionary.TryGetValue(key, out value))                
                dictionary.Add(key, value = new List<int>());
            
            return value;            
        }        
    }
}