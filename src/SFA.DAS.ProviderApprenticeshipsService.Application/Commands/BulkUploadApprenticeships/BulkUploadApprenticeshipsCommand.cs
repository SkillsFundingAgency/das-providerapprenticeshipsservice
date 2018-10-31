using System.Collections.Generic;
using MediatR;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommand : IRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }
        public IList<Apprenticeship> Apprenticeships { get; set; }
        public string UserEmailAddress { get; set; }
        public string UserDisplayName { get; set; }
    }
}
