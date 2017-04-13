using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.ReviewApprenticeshipUpdate
{
    public sealed class ReviewApprenticeshipUpdateCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }
        public bool IsApproved { get; set; }
    }
}
