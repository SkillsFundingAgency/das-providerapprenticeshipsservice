using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string Ukprn { get; set; }
        public string Email { get; set; }
    }
}
