using System.Linq;
using System.Threading.Tasks;

using FluentValidation;
using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAccountUsers
{
    public class GetAccountUsersHandler : IAsyncRequestHandler<GetAccountUsersQuery, GetAccountUsersResponse>
    {
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly IUserRepository _userRepository;

        public GetAccountUsersHandler(IUserSettingsRepository userSettingsRepository, IUserRepository userRepository)
        {
            _userSettingsRepository = userSettingsRepository;
            _userRepository = userRepository;
        }

        public async  Task<GetAccountUsersResponse> Handle(GetAccountUsersQuery request)
        {
            if(request.Ukprn < 1)
                throw new ValidationException($"Ukprn must be more than 0 when getting account users.");

            var response = new GetAccountUsersResponse();
            var providerUsers = await _userRepository.GetUsers(request.Ukprn);
            foreach (var user in providerUsers)
            {
                var settings = await _userSettingsRepository.GetUserSetting(user.UserRef);
                response.Add(user, settings.FirstOrDefault());
            }

            return response;
        }
    }
}