using System.Text.RegularExpressions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveHtmlTags(this string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }
    }
}