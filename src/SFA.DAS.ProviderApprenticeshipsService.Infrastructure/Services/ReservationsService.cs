using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.LocalDev;
using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Api.Types;
using SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.BulkValidate;

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

        public Task<bool> IsAutoReservationEnabled(long accountId, long? transferSenderId)
        {
            return IsAutoReservationEnabledWithLog(accountId, transferSenderId, CancellationToken.None);
        }

        public Task<bool> IsAutoReservationEnabled(long accountId, long? transferSenderId, CancellationToken cancellationToken)
        {
            return IsAutoReservationEnabledWithLog(accountId, transferSenderId, cancellationToken);
        }

        public async Task<Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults> GetReservationErrors(IEnumerable<Reservations.Api.Models.Reservation> reservations)
        {
            var client = (_reservationsApiClient as ReservationsApiClient2);
            var result = await client.BulkValidate(reservations, CancellationToken.None);
            return result;
        }

        public async Task<Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults> GetReservationErrors(Reservations.Api.Models.Reservation reservation)
        {
            var client = (_reservationsApiClient as ReservationsApiClient2);
            var result = await client.BulkValidate(reservation, CancellationToken.None).ConfigureAwait(false);
            return result;
        }

        private async Task<bool> IsAutoReservationEnabledWithLog(long accountId, long? transferSenderId, CancellationToken cancellationToken)
        {
            try
            {
                var request = new ReservationAllocationStatusMessage
                {
                    AccountId = accountId,
                    TransferSenderId = transferSenderId
                };

                var result = await _reservationsApiClient.GetReservationAllocationStatus(request, cancellationToken);
                return true;
                //return result.CanAutoCreateReservations;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Getting auto reservation status for account {accountId}");
                throw;
            }
        }
    }
}