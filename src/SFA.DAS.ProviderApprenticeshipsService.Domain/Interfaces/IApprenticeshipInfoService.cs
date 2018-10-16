using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IApprenticeshipInfoService
    {
        Task<StandardsView> GetStandardsAsync(bool refreshCache = false);
        Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false);
        ProvidersView GetProvider(long ukprn);
    }
}