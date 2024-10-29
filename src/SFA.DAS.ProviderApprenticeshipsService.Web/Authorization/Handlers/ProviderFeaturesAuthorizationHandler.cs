using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Errors;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Services;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization.Handlers;

public class ProviderFeaturesAuthorizationHandler(IFeatureTogglesService<FeatureToggle> featureTogglesService)
    : IAuthorizationHandler
{
    public string Prefix => "ProviderFeature.";

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
            authorizationResult.AddError(new ProviderFeatureNotEnabled());
        }
        else if (featureToggle.IsWhitelistEnabled)
        {
            var values = authorizationContext.GetProviderFeatureValues();

            if (!featureToggle.IsUserWhitelisted(values.Ukprn, values.UserEmail))
            {
                authorizationResult.AddError(new ProviderFeatureUserNotWhitelisted());
            }
        }

        return Task.FromResult(authorizationResult);
    }
}