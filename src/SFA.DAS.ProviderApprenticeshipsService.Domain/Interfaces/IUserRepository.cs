using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task Upsert(User user);
    }
}
