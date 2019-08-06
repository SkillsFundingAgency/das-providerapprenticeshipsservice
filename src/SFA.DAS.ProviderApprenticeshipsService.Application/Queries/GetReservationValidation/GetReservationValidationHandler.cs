using MediatR;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Reservations.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetReservationValidation
{
    public class GetReservationValidationHandler : IRequestHandler<GetReservationValidationRequest, GetReservationValidationResponse>
    {
        private readonly IReservationsApiClient _reservationClient;

        public GetReservationValidationHandler(IReservationsApiClient reservationClient)
        {
            _reservationClient = reservationClient;
        }

        public async Task<GetReservationValidationResponse> Handle(GetReservationValidationRequest request, CancellationToken cancellationToken)
        {
            var validationReservationMessage = new ValidationReservationMessage
            {
                AccountId = request.AccountId,
                StartDate = request.StartDate,
                CourseCode = request.TrainingCode,
                ReservationId = request.ReservationId
            };

            var result = await _reservationClient.ValidateReservation(validationReservationMessage, CancellationToken.None);

            return new GetReservationValidationResponse
            {
                Data = result
            };
        }
    }
}