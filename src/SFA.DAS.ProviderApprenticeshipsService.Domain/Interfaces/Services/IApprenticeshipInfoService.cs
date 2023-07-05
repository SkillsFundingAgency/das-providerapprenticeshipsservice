using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Services;

public interface IApprenticeshipInfoService
{
    Task<ProvidersView> GetProvider(long ukprn);
}