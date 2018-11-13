using MediatR;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Commands.UpdateRelationship
{
    public class UpdateRelationshipCommand: IRequest
    {
        public long ProviderId { get; set; }
        public string UserId { get; set; }
        public Relationship Relationship { get; set; }
    }
}
