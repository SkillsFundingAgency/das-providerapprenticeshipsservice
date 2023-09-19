using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
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
        private readonly ITrainingProviderService _trainingProviderService;

        public TrainingProviderAuthorizationHandler(
            IHttpContextAccessor httpContextAccessor,
            ITrainingProviderService trainingProviderService)
        {
            _httpContextAccessor = httpContextAccessor;
            _trainingProviderService = trainingProviderService;
        }

        public async Task<bool> IsProviderAuthorized(AuthorizationHandlerContext context, bool allowAllUserRoles)
        {
            var ukprn = GetProviderId();
            var providerDetails = await _trainingProviderService.GetProviderDetails(ukprn);

            // Logic to check if the provider is a valid
            // Condition 1: is the provider's profile a Main or Employer Profile.
            // Condition 2: is the provider's status Active or On-boarding.
            return providerDetails is
            {
                ProviderTypeId: (int)ProviderTypeIdentifier.MainProvider or (int)ProviderTypeIdentifier.EmployerProvider,
                StatusId: (int)ProviderStatusType.Active or (int)ProviderStatusType.Onboarding
            };
        }

        private long GetProviderId()
        {
            if (long.TryParse(_httpContextAccessor.HttpContext?.User.Identity.GetClaim(DasClaimTypes.Ukprn), out var providerId))
                return providerId;

            throw new InvalidOperationException("AuthorizationContextProvider error - Unable to extract ProviderId");
        }
    }
}
