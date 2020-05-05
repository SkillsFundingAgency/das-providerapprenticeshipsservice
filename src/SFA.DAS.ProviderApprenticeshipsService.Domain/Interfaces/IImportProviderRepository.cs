using System.Data;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IImportProviderRepository
    {
        Task ImportProviders(DataTable providersDataTable);
    }
}
