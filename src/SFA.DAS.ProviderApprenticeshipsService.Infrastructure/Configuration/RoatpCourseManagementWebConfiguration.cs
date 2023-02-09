using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    public class RoatpCourseManagementWebConfiguration : IRoatpCourseManagementWebConfiguration
    {
        public ProviderFeaturesConfiguration ProviderFeaturesConfiguration { get; set; }
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
    public class ProviderFeaturesConfiguration
    {
        public List<ProviderFeatureToggle> FeatureToggles { get; set; }
    }

    public class ProviderFeatureToggle : FeatureToggle
    {
        public List<ProviderFeatureToggleWhitelistItem> Whitelist { get; set; }
    }

    public class FeatureToggle
    {
        public string Feature { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class ProviderFeatureToggleWhitelistItem
    {
        public int Ukprn { get; set; }
    }
}
