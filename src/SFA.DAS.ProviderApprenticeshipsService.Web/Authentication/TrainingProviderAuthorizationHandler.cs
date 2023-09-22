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
    public class TrainingProviderAuthorizationHandler : ITrainingProviderAuthorizationHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITrainingProviderApiClient _trainingProviderApiClient;

        public TrainingProviderAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            ITrainingProviderApiClient trainingProviderApiClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _trainingProviderApiClient = trainingProviderApiClient;
        }

        public async Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context, bool allowAllUserRoles)
        {
            var ukprn = GetProviderId();
            var providerStatus = await _trainingProviderApiClient.GetProviderStatus(ukprn);
            return providerStatus is { IsValidProvider: true };
        }

        private long GetProviderId()
        {
            if (long.TryParse(_httpContextAccessor.HttpContext?.User.Identity.GetClaim(DasClaimTypes.Ukprn), out var providerId))
                return providerId;

            throw new InvalidOperationException("AuthorizationContextProvider error - Unable to extract ProviderId");
        }
    }
}
