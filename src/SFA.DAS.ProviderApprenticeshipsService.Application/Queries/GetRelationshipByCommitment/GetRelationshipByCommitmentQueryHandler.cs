using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetRelationshipByCommitment
{
    public class GetRelationshipByCommitmentQueryHandler : IRequestHandler<GetRelationshipByCommitmentQueryRequest, GetRelationshipByCommitmentQueryResponse>
    {
        private readonly IRelationshipApi _relationshipApi;

        public GetRelationshipByCommitmentQueryHandler(IRelationshipApi relationshipApi)
        {
            _relationshipApi = relationshipApi;
        }

        public async Task<GetRelationshipByCommitmentQueryResponse> Handle(GetRelationshipByCommitmentQueryRequest message, CancellationToken cancellationToken)
        {
            var response = await _relationshipApi.GetRelationshipByCommitment(message.ProviderId, message.CommitmentId);

            return new GetRelationshipByCommitmentQueryResponse
            {
                Relationship = response
            };
        }
    }
}
