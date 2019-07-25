using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetReservationValidation
{
    public class GetReservationValidationHandler : IRequestHandler<GetReservationValidationRequest, GetReservationValidationResponse>
    {
        private readonly IValidationApi _validationApi;

        public GetReservationValidationHandler(IValidationApi validationApi)
        {
            _validationApi = validationApi;
        }

        public async Task<GetReservationValidationResponse> Handle(GetReservationValidationRequest request, CancellationToken cancellationToken)
        {
            var validationRequest = new ReservationValidationRequest
            {
                ApprenticeshipId = request.ApprenticeshipId,
                ProposedCourseCode = request.ProposedTrainingCode,
                ProposedStartDate = request.ProposedStartDate
            };

            var result = await _validationApi.ValidateReservation(validationRequest);

            return new GetReservationValidationResponse
            {
                Data = result
            };
        }
    }
}