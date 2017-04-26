using System;

using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.RequestApprenticeshipRestart
{
    public sealed class RestartApprenticeshipCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public long ApprenticeshipId { get; set; }
    }
}