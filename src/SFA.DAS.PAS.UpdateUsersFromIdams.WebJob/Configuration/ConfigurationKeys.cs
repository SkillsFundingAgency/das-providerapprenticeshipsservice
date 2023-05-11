using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.Configuration
{
    public static class ConfigurationKeys
    {
        public const string ProviderApprenticeshipsService = "SFA.DAS.ProviderApprenticeshipsService";
        public const string CommitmentNotification = $"{ProviderApprenticeshipsService}:CommitmentNotification";
    }
}
