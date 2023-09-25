using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
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

            // logic to check if the provider is authorized if not redirect the user to 401 un-authorized page.
            if (isStubProviderValidationEnabled && !(await _handler.IsProviderAuthorized(context, true)))
            {
                var mvcContext = context.Resource as DefaultHttpContext;
                mvcContext?.Response.Redirect("/error/401");
            }

            context.Succeed(requirement);
        }

        private bool GetUseStubProviderValidationSetting()
        {
            var value = _configuration.GetSection("UseStubProviderValidation").Value;

            return value != null && bool.TryParse(value, out var result) && result;
        }
    }
}
