using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteApprenticeship
{
    public sealed class DeleteApprenticeshipCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserDisplayName { get; set; }
    }
}
