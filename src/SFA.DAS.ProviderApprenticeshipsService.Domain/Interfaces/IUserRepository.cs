using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task Upsert(User user);

        Task<User> GetUser(string userRef);

        Task<IEnumerable<User>>  GetUsers(long ukprn);
    }
}
