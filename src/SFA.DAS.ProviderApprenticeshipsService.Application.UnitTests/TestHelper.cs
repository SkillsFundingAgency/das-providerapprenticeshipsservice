﻿using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using Newtonsoft.Json;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.UnitTests
{
    public static class TestHelper
    {
        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public static bool EnumerablesAreEqual(IEnumerable<object> expected, IEnumerable<object> actual)
        {
            return new CompareLogic(new ComparisonConfig { IgnoreObjectTypes = true })
                .Compare(expected, actual).AreEqual;
        }
    }
}
