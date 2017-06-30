using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class AuthenticationOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly IProviderCommitmentsLogger _logger;

        public AuthenticationOrchestrator(IMediator mediator, IProviderCommitmentsLogger logger)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _mediator = mediator;
            _logger = logger;
        }

        public async Task SaveIdentityAttributes(string userId, string ukprn, string displayName, string email)
        {
            _logger.Info($"Updating \"{userId}\" attributes - ukprn:\"{ukprn}\", displayname:\"{displayName}\", email:\"{email}\"");

            await _mediator.SendAsync(new UpsertRegisteredUserCommand
            {
                UserId = userId,
                DisplayName = displayName,
                Ukprn = ukprn,
                Email = email
            });
        }
    }
}