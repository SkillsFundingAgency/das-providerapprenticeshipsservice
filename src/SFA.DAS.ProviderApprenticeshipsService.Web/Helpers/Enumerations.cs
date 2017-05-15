using System;
using System.ComponentModel;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Helpers
{
    public static class Enumerations
    {
        public static string GetDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());

            var attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}