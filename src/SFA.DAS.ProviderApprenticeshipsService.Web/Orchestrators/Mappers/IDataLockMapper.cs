using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.DataLock;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public interface IDataLockMapper
    {
        Task<DataLockSummaryViewModel> MapDataLockSummary(DataLockSummary datalockSummary);

        Task<DataLockViewModel> MapDataLockStatus(DataLockStatus dataLock);

        Task<List<DataLockViewModel>> MapDataLockStatusList(List<DataLockStatus> datalocks);
    }
}
