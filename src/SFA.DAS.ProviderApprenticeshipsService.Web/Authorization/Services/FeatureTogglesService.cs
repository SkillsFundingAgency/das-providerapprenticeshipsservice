using System.Collections.Concurrent;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

public interface IFeatureTogglesService<T> where T : FeatureToggle, new()
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
        if (configuration?.FeatureToggles == null)
        {
            _featureToggles = new ConcurrentDictionary<string, TFeatureToggle>();
        }
        else
        {
            _featureToggles = new ConcurrentDictionary<string, TFeatureToggle>(configuration.FeatureToggles.ToDictionary(t => t.Feature));
        }
    }

    public TFeatureToggle GetFeatureToggle(string feature)
    {
        return _featureToggles.GetOrAdd(feature, f => new TFeatureToggle { Feature = f, IsEnabled = false });
    }
}