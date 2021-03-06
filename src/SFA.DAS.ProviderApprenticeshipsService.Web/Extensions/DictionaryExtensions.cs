﻿using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddIfNotExists<T1, T2>(this Dictionary<T1, T2> self, T1 key, T2 value)
        {
            if (!self.ContainsKey(key))
                self.Add(key, value);
        }

        public static void AddIfNotExists<T1, T2>(this Dictionary<T1, T2> self, KeyValuePair<T1, T2> keyValuePair)
        {
            self.AddIfNotExists(keyValuePair.Key, keyValuePair.Value);
        }
    }
}