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
            return IsAutoReservationEnabledWithLog(accountId);
        }

        public Task<bool> IsAutoReservationEnabledWithLog(long accountId)
        {
            try
            {
                return IsAutoReservationEnabled(accountId, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Getting auto reservation status for account {accountId}");
                throw;
            }
        }

        public async Task<bool> IsAutoReservationEnabled(long accountId, CancellationToken cancellationToken)
        {
            var request = new ReservationAllocationStatusMessage { AccountId = accountId };

            var result = await _reservationsApiClient.GetReservationAllocationStatus(request, cancellationToken);

            return result.AutoReservations;
        }
    }
}
