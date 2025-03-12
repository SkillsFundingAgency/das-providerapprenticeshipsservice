using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authentication
{
    /// <summary>
    /// Interface to define contracts related to Training Provider Authorization Handlers.
    /// </summary>
    public interface ITrainingProviderAuthorizationHandler
    {
        /// <summary>
        /// Contract to check is the logged in Provider is a valid Training Provider. 
        /// </summary>
        /// <param name="context">AuthorizationHandlerContext.</param>
        /// <param name="allowAllUserRoles">boolean.</param>
        /// <returns>boolean.</returns>
        Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context, bool allowAllUserRoles);
    }

    // <inherit-doc />
    public class TrainingProviderAuthorizationHandler(
        ILogger<TrainingProviderAuthorizationHandler> logger,
        ITrainingProviderApiClient trainingProviderApiClient)
        : ITrainingProviderAuthorizationHandler
    {
        // <inherit-doc />
        public async Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context, bool allowAllUserRoles)
        {
            logger.LogInformation("Logged in claims: {Claims}", JsonSerializer.Serialize(context.User.Claims));
            if (!long.TryParse(context.User.Identity.GetClaim(DasClaimTypes.Ukprn),
                    out var ukprn))
            {
                return false;
            }
            
            var providerDetails = await trainingProviderApiClient.GetProviderDetails(ukprn);

            // Condition to check if the Provider Details has permission to access Apprenticeship Services based on the property value "CanAccessApprenticeshipService" set to True.
            return providerDetails is { CanAccessApprenticeshipService: true };
        }
    }
}
