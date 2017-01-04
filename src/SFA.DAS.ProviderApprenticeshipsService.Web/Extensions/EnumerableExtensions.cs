using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value == null || !value.Any();
        }
    }
}