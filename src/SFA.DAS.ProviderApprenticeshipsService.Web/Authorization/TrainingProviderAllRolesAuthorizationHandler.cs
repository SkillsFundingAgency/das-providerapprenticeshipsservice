﻿using Microsoft.AspNetCore.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Authorization
{
    public class TrainingProviderAllRolesAuthorizationHandler : AuthorizationHandler<TrainingProviderAllRolesRequirement>
    {
        private readonly ITrainingProviderAuthorizationHandler _handler;

        public TrainingProviderAllRolesAuthorizationHandler(ITrainingProviderAuthorizationHandler handler)
        {
            _handler = handler;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, TrainingProviderAllRolesRequirement requirement)
        {
            // logic to check if the provider is authorized if not redirect the user to 401 un-authorized page.
            if (!(await _handler.IsProviderAuthorized(context, true)))
            {
                var mvcContext = context.Resource as DefaultHttpContext;
                mvcContext?.Response.Redirect("/error/401");
            }

            context.Succeed(requirement);
        }
    }
}
