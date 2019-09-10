using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.Reservations.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class ReservationsService : IReservationsService
    {
        private readonly IReservationsApiClient _reservationsApiClient;
        private readonly ILog _logger;

        public ReservationsService(IReservationsApiClient reservationsApiClient, ILog logger)
        {
            _reservationsApiClient = reservationsApiClient;
            _logger = logger;
        }

        public Task<bool> IsAutoReservationEnabled(long accountId)
        {
            return IsAutoReservationEnabledWithLog(accountId, CancellationToken.None);
        }

        public Task<bool> IsAutoReservationEnabled(long accountId, CancellationToken cancellationToken)
        {
            return IsAutoReservationEnabledWithLog(accountId, cancellationToken);
        }

        private async Task<bool> IsAutoReservationEnabledWithLog(long accountId, CancellationToken cancellationToken)
        {
            try
            {
                var request = new ReservationAllocationStatusMessage { AccountId = accountId };

                var result = await _reservationsApiClient.GetReservationAllocationStatus(request, cancellationToken);

                return result.CanAutoCreateReservations;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Getting auto reservation status for account {accountId}");
                throw;
            }
        }
    }
}
