using System.Threading.Tasks;

using MediatR;

using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Api.Orchestrator
{
    public class UserOrchestrator
    {
        private readonly IMediator _mediator;

        private readonly IProviderCommitmentsLogger _logger;

        public UserOrchestrator(IMediator mediator, IProviderCommitmentsLogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<User> GetUser(string userRef)
        {
            var userSetting = await _mediator.SendAsync(new GetUserRequest {UserRef = userRef });

            if (userSetting == null)
            {
                _logger.Info($"Unable to get user with ref {userRef}");
                return null;
            }

            return new User { UserRef = userSetting.UserRef, ReceiveNotifications = userSetting.ReceiveNotifications };
        }
    }
}