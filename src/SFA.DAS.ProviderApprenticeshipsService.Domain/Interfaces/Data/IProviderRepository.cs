using System.Threading.Tasks;
using Provider = SFA.DAS.ProviderApprenticeshipsService.Domain.Models.Provider;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

public interface IProviderRepository
{
    Task ImportProviders(CommitmentsV2.Api.Types.Responses.Provider[] providers);
    Task<Provider> GetNextProviderForIdamsUpdate();
    Task MarkProviderIdamsUpdated(long ukprn);
    Task<Provider> GetProvider(long ukprn);
}