using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.TrainingProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services
{
    public interface ITrainingProviderApiClient
    {
        Task<GetProviderStatusResult> GetProviderStatus(long providerId);
    }
}
