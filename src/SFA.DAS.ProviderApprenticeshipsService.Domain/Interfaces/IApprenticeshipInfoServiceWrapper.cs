using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IApprenticeshipInfoServiceWrapper
    {
        Task<StandardsView> GetStandardsAsync(bool refreshCache = false);
        Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false);
        ProvidersView GetProvider(long ukPrn);
    }
}