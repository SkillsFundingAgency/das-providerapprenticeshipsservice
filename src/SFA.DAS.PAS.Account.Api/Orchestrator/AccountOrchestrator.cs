using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

namespace SFA.DAS.PAS.Account.Api.Orchestrator
{
    public class AccountOrchestrator : IAccountOrchestrator
    {
        private readonly IMediator _mediator;

        private readonly IProviderCommitmentsLogger _logger;

        public AccountOrchestrator(IMediator mediator, IProviderCommitmentsLogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAccountUsers(long ukprn)
        {
            var result = await _mediator.Send(new GetAccountUsersQuery { Ukprn = ukprn });

            return result.UserSettings.Select(
                m =>
                new User { EmailAddress = m.User.Email, DisplayName = m.User.DisplayName, ReceiveNotifications = m.Setting?.ReceiveNotifications ?? true, UserRef = m.User.UserRef, 
                    IsSuperUser = m.User.UserType == UserType.SuperUser });
        }

        public async Task<ProviderAgreement> GetAgreement(long providerId)
        {
            var data = await _mediator.Send(
                new GetProviderAgreementQueryRequest
                {
                    ProviderId = providerId
                });

            return new ProviderAgreement
            {
                Status = (ProviderAgreementStatus)Enum.Parse(typeof(ProviderAgreementStatus), data.HasAgreement.ToString())
            };
        }
    }
}