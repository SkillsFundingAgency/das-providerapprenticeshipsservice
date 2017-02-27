using System.Collections.Generic;
using MediatR;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships
{
    public sealed class BulkUploadApprenticeshipsCommand : IAsyncRequest
    {
        public string UserId { get; set; }
        public long ProviderId { get; set; }
        public long CommitmentId { get; set; }
        public IList<Apprenticeship> Apprenticeships { get; set; }
    }
}
