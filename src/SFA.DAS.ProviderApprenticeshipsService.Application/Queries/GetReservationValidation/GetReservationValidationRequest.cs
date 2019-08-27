using System;
using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetReservationValidation
{
    public class GetReservationValidationRequest : IRequest<GetReservationValidationResponse>
    {
        public long AccountId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string CourseCode { get; set; }
        public DateTime StartDate { get; set; }
        public Guid ReservationId { get; set; }
    }
}
