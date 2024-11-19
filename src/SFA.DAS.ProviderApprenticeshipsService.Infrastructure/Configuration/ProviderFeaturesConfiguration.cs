using System.Collections.Generic;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

public interface IFeaturesConfiguration<T> where T : FeatureToggle, new()
{
    List<T> FeatureToggles { get; }
}

public class ProviderFeaturesConfiguration : IFeaturesConfiguration<ProviderFeatureToggle>
{
    public List<ProviderFeatureToggle> FeatureToggles { get; set; }
}