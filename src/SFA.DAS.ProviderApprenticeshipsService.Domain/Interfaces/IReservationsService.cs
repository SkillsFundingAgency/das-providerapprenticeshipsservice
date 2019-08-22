using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IReservationsService
    {
        Task<bool> IsAutoReservationEnabled(long accountId);
        Task<bool> IsAutoReservationEnabled(long accountId, CancellationToken cancellationToken);
    }
}
