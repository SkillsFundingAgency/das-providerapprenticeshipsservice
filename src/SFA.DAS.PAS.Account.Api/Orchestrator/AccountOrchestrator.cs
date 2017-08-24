﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Orchestrator
{
    public class AccountOrchestrator
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
            var result = await _mediator.SendAsync(new GetAccountUsersQuery { Ukprn = ukprn });

            return result.UserSettings.Select(
                m =>
                new User { EmailAddress = m.User.Email, ReceiveNotifications = m.Setting.ReceiveNotifications, UserRef = m.User.UserRef });
        }
    }
}