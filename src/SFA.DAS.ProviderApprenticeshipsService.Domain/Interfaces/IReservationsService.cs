using SFA.DAS.Reservations.Api.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IReservationsService
    {
        Task<bool> IsAutoReservationEnabled(long accountId, long? transferSenderId);
        Task<bool> IsAutoReservationEnabled(long accountId, long? transferSenderId, CancellationToken cancellationToken);
        Task<Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults> GetReservationErrors(IEnumerable<Reservation> reservations);
        Task<Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults> GetReservationErrors(Reservation reservations);
    }
}
