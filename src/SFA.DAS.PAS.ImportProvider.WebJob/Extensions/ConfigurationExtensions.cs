﻿using Microsoft.Extensions.Configuration;
using System;

namespace SFA.DAS.PAS.ImportProvider.WebJob.Extensions
{
    public static class ConfigurationExtensions
    {
        public static bool IsDev(this IConfiguration configuration)
        {
            return configuration["EnvironmentName"].Equals("Development", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsLocal(this IConfiguration configuration)
        {
            return configuration["EnvironmentName"].StartsWith("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
