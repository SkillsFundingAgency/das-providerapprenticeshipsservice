using System.Linq;
using System.Threading.Tasks;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetUser
{
    public class GetUserHandler : IAsyncRequestHandler<GetUserRequest, GetUserResponse>
    {
        private readonly IUserSettingsRepository _userSettingsRepository;

        public GetUserHandler(IUserSettingsRepository userSettingsRepository)
        {
            _userSettingsRepository = userSettingsRepository;
        }

        public async Task<GetUserResponse> Handle(GetUserRequest request)
        {
            var userSetting = await _userSettingsRepository.GetUserSetting(request.UserRef);
            var setting = userSetting.FirstOrDefault();
            if (setting != null)
            {
                return new GetUserResponse
                           {
                               UserRef = setting.UserRef,
                               ReceiveNotifications = setting.ReceiveNotifications
                           };
            }

            return null;
        }
    }
}