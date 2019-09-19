using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IReservationsService
    {
        Task<bool> IsAutoReservationEnabled(long accountId, long? transferSenderId);
        Task<bool> IsAutoReservationEnabled(long accountId, long? transferSenderId, CancellationToken cancellationToken);
    }
}
