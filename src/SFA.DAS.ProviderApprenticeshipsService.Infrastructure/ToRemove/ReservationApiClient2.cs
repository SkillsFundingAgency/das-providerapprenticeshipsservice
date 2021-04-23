using SFA.DAS.Reservations.Api.Models;
using SFA.DAS.Reservations.Api.Types;
using SFA.DAS.Reservations.Api.Types.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SFA.DAS.ProviderApprenticeshipsService.Web.LocalDev
{
    public class ReservationsApiClient2 : ReservationsApiClient
    {
        private readonly ReservationsClientApiConfiguration _config;
        private readonly IHttpHelper _httpHelper;

        public ReservationsApiClient2(ReservationsClientApiConfiguration config, IHttpHelper httpHelper) : base(config, httpHelper)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _httpHelper = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));
        }

        public Task<SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults> BulkValidate(IEnumerable<Reservation> request, CancellationToken cancellationToken)
        {
            var url = BuildUrl($"api/Reservations/accounts/{request.First().AccountLegalEntityId}/bulk-validate");
          

            return _httpHelper.PostAsJson<IEnumerable<Reservation>, Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults>(url, request, cancellationToken);
        }

        public Task<SFA.DAS.Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults> BulkValidate(Reservation request, CancellationToken cancellationToken)
        {
            var url = BuildUrl($"api/Reservations/accounts/{request.AccountLegalEntityId}/bulk-validate/reservation");
            return _httpHelper.PostAsJson<Reservation, Reservations.Application.AccountLegalEntities.Queries.BulkValidate.BulkValidationResults>(url, request, cancellationToken);
        }

        private string BuildUrl(string path)
        {
            var effectiveApiBaseUrl = _config.EffectiveApiBaseUrl.TrimEnd(new[] { '/' });
            path = path.TrimStart(new[] { '/' });

            return $"{effectiveApiBaseUrl}/{path}";
        }
    }
}





