using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers
{
    public class GetAccountUsersHandler : IRequestHandler<GetAccountUsersQuery, GetAccountUsersResponse>
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IUserRepository _userRepository;

        private readonly IProviderCommitmentsLogger _logger;

        public GetAccountUsersHandler(
            IUserSettingsRepository userSettingsRepository, 
            IUserRepository userRepository,
            IProviderCommitmentsLogger logger)
        {
            _userSettingsRepository = userSettingsRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<GetAccountUsersResponse> Handle(GetAccountUsersQuery request, CancellationToken cancellationToken)
        {
            if(request.Ukprn < 1)
                throw new ValidationException($"Ukprn must be more than 0 when getting account users.");

            var response = new GetAccountUsersResponse();
            _logger.Info($"Getting users from repository for {request.Ukprn}", providerId:request.Ukprn);
            var providerUsers = await _userRepository.GetUsers(request.Ukprn);
            foreach (var user in providerUsers)
            {
                var settings = await _userSettingsRepository.GetUserSetting(user.UserRef);
                response.Add(user, settings.FirstOrDefault());
            }

            _logger.Info($"Retrieved {providerUsers.Count()} users from repository for {request.Ukprn}", providerId: request.Ukprn);

            return response;
        }
    }
}