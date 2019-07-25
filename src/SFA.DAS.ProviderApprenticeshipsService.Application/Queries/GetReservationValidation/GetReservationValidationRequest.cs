using System;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetReservationValidation
{
    public class GetReservationValidationRequest : IRequest<GetReservationValidationResponse>
    {
        public long ApprenticeshipId { get; set; }
        public string ProposedTrainingCode { get; set; }
        public DateTime? ProposedStartDate { get; set; }
    }
}
