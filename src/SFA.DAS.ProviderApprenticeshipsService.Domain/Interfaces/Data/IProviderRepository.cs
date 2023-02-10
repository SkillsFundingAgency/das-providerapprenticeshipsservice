using System.Data;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderRepository
    {
        Task ImportProviders(DataTable providersDataTable);
        Task<Provider> GetNextProviderForIdamsUpdate();
        Task MarkProviderIdamsUpdated(long ukprn);
    }
}
