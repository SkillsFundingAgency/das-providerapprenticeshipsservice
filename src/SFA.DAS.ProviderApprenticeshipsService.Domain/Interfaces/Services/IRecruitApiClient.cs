using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services
{
    public interface IRecruitApiClient
    {
        Task<TResponse> Get<TResponse>(IGetApiRequest request);
    }
}
