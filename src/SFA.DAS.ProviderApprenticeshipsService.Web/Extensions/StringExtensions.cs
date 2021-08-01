using System.Text.RegularExpressions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveHtmlTags(this string source)
        {
            return Regex.Replace(source, "<.*?>", string.Empty);
        }

        public static int? TryParse(this string input)
        {
            int val;
            if(int.TryParse(input, out val))
            {
                return val;
            }

            return null;
        }

        public static decimal? AsNullableDecimal(this string input)
        {
            var result = default(decimal?);
            decimal parsed;
            if (decimal.TryParse(input, out parsed))
            {
                result = parsed;
            }

            return result;
        }

        public static bool IsAValidEmailAddress(this string emailAsString)
        {
            try
            {
                var email = new System.Net.Mail.MailAddress(emailAsString);

                // check it contains a top level domain
                var parts = email.Address.Split('@');
                if (!parts[1].Contains(".") || parts[1].EndsWith("."))
                {
                    return false;
                }

                return email.Address == emailAsString;
            }
            catch
            {
                return false;
            }
        }
    }
}