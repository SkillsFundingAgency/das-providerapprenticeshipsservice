using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.FeatureToggles;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

public class AuthorizationHandler(IFeatureTogglesService<FeatureToggle> featureTogglesService) : IAuthorizationHandler
{
    public string Prefix => "Feature.";

    public Task<AuthorizationResult> GetAuthorizationResult(IReadOnlyCollection<string> options, IAuthorizationContext authorizationContext)
    {
        var authorizationResult = new AuthorizationResult();

        if (options.Count <= 0)
        {
            return Task.FromResult(authorizationResult);
        }

        options.EnsureNoAndOptions();
        options.EnsureNoOrOptions();

        var feature = options.Single();
        var featureToggle = featureTogglesService.GetFeatureToggle(feature);

        if (!featureToggle.IsEnabled)
        {
            authorizationResult.AddError(new FeatureNotEnabled());
        }

        return Task.FromResult(authorizationResult);
    }
}