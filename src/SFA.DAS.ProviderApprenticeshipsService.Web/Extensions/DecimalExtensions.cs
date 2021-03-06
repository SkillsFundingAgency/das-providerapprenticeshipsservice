﻿namespace SFA.DAS.ProviderApprenticeshipsService.Web.Extensions
{
    public static class DecimalExtensions
    {
        public static string FormatCost(this decimal? value)
        {
            return !value.HasValue ? string.Empty : $"£{value.Value:n0}";
        }

        public static string FormatCost(this decimal value)
        {
            return $"£{value:n0}";
        }
    }
}