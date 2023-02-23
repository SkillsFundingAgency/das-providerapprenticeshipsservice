using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderRepository
    {
        Task ImportProviders(ProviderResponse[] providerResponses);
        Task<Provider> GetNextProviderForIdamsUpdate();
        Task MarkProviderIdamsUpdated(long ukprn);
    }
}
