using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IApprenticeshipInfoService
    {
        Task<StandardsView> GetStandards(bool refreshCache = false);
        Task<AllTrainingProgrammesView> GetAll(bool refreshCache = false);
        Task<ProvidersView> GetProvider(long ukprn);
    }
}