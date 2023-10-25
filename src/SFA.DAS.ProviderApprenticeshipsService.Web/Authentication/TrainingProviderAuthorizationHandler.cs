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
    public class TrainingProviderAuthorizationHandler : ITrainingProviderAuthorizationHandler
    {
        private readonly ITrainingProviderApiClient _trainingProviderApiClient;

        public TrainingProviderAuthorizationHandler(
            ITrainingProviderApiClient trainingProviderApiClient)
        {
            _trainingProviderApiClient = trainingProviderApiClient;
        }

        // <inherit-doc />
        public async Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context, bool allowAllUserRoles)
        {
            if (!long.TryParse(context.User.Identity.GetClaim(DasClaimTypes.Ukprn),
                    out var ukprn))
            {
                return false;
            }
            
            var providerDetails = await _trainingProviderApiClient.GetProviderDetails(ukprn);

            // Condition to check if the Provider Details has permission to access Apprenticeship Services based on the property value "CanAccessApprenticeshipService" set to True.
            return providerDetails is { CanAccessApprenticeshipService: true };
        }
    }
}
