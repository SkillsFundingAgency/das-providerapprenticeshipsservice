using System.Collections.Concurrent;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.FeatureToggles;

public interface IFeatureTogglesService<out T> where T : FeatureToggle, new()
{
    T GetFeatureToggle(string feature);
}

public class FeatureTogglesService<TConfiguration, TFeatureToggle> : IFeatureTogglesService<TFeatureToggle>
    where TConfiguration : IFeaturesConfiguration<TFeatureToggle>, new()
    where TFeatureToggle : FeatureToggle, new()
{
    private readonly ConcurrentDictionary<string, TFeatureToggle> _featureToggles;

    public FeatureTogglesService(TConfiguration configuration)
    {
        _featureToggles = configuration?.FeatureToggles == null 
            ? new ConcurrentDictionary<string, TFeatureToggle>() 
            : new ConcurrentDictionary<string, TFeatureToggle>(configuration.FeatureToggles.ToDictionary(t => t.Feature));
    }

    public TFeatureToggle GetFeatureToggle(string feature)
    {
        return _featureToggles.GetOrAdd(feature, f => new TFeatureToggle { Feature = f, IsEnabled = false });
    }
}