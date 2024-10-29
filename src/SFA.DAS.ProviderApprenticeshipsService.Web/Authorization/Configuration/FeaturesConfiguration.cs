using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Configuration;

public interface IFeaturesConfiguration<T> where T : FeatureToggle, new()
{
    List<T> FeatureToggles { get; }
}

public class FeaturesConfiguration : IFeaturesConfiguration<FeatureToggle>
{
    public List<FeatureToggle> FeatureToggles { get; set; }
}