using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.PAS.Account.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUserNotificationSettings;
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
            var userSetting = await _mediator.SendAsync(new GetUserNotificationSettingsQuery {UserRef = userRef });

            var setting = userSetting.NotificationSettings.SingleOrDefault();
            if (setting == null)
            {
                _logger.Info($"Unable to get user with ref {userRef}");
                return null;
            }

            return new User { UserRef = setting.UserRef, ReceiveNotifications = setting.ReceiveNotifications };
        }
    }
}