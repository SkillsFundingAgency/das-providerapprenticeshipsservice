using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IClientContentService
    {
        Task<string> Get(string type, string applicationId);
    }
}
