using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.UserProfile;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Services.UserIdentityService
{
    public interface IUserIdentityService
    {
        Task<Unit> UpsertUserIdentityAttributes(string userId, long ukprn, string displayName, string email);
    }

    public class UserIdentityService : IUserIdentityService
    {
        private readonly IUserRepository _userRepository;

        public UserIdentityService(IUserRepository userRepository)
        {
            _userRepository= userRepository;
        }

        public async Task<Unit> UpsertUserIdentityAttributes(string userId, long ukprn, string displayName, string email)
        {
            await _userRepository.Upsert(new User
            {
                UserRef = userId,
                DisplayName = displayName,
                Email = email,
                Ukprn = ukprn,
                IsDeleted = false
            });

            return Unit.Value;
        }
    }
}
