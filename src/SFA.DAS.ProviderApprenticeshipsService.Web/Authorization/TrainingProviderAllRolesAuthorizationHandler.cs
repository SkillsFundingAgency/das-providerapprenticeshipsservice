using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    /// <summary>
    /// AuthorizationHandler to validate the Training Provider and it applies to all Provider roles.
    /// </summary>
    public class TrainingProviderAllRolesAuthorizationHandler : AuthorizationHandler<TrainingProviderAllRolesRequirement>
    {
        private readonly ITrainingProviderAuthorizationHandler _handler;
        private readonly IConfiguration _configuration;

        public TrainingProviderAllRolesAuthorizationHandler(ITrainingProviderAuthorizationHandler handler, IConfiguration configuration)
        {
            _handler = handler;
            _configuration = configuration;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TrainingProviderAllRolesRequirement requirement)
        {
            var isStubProviderValidationEnabled = GetUseStubProviderValidationSetting();

            // check if the stub is activated to by-pass the validation. Mostly used for local development purpose.
            // logic to check if the provider is authorized if not redirect the user to 401 un-authorized page.
            if (!isStubProviderValidationEnabled && !(await _handler.IsProviderAuthorized(context, true)))
            {
                var mvcContext = context.Resource as DefaultHttpContext;
                mvcContext?.Response.Redirect("/error/403/invalid-status");
            }

            context.Succeed(requirement);
        }

        #region "Private Methods"
        /// <summary>
        /// Read the Stub value from the Configuration.
        /// </summary>
        /// <returns>boolean.</returns>
        private bool GetUseStubProviderValidationSetting()
        {
            var value = _configuration.GetSection("UseStubProviderValidation").Value;

            return value != null && bool.TryParse(value, out var result) && result;
        }
        #endregion
    }
}
