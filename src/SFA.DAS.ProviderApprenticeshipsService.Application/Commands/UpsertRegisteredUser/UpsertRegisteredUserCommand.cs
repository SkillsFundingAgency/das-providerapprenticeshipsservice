using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpsertRegisteredUser
{
    public class UpsertRegisteredUserCommand : IAsyncRequest
    {
        public string UserRef { get; set; }
        public string DisplayName { get; set; }
        public long Ukprn { get; set; }
        public string Email { get; set; }
    }
}
