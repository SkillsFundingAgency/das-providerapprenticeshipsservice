using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetRelationshipByCommitment
{
    public class GetRelationshipByCommitmentQueryHandler : IAsyncRequestHandler<GetRelationshipByCommitmentQueryRequest, GetRelationshipByCommitmentQueryResponse>
    {
        private readonly ICommitmentsApi _commitmentsApi;

        public GetRelationshipByCommitmentQueryHandler(ICommitmentsApi commitmentsApi)
        {
            if (commitmentsApi == null)
                throw new ArgumentNullException(nameof(commitmentsApi));
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetRelationshipByCommitmentQueryResponse> Handle(GetRelationshipByCommitmentQueryRequest message)
        {
            var response = await _commitmentsApi.GetRelationshipByCommitment(message.ProviderId, message.CommitmentId);

            return new GetRelationshipByCommitmentQueryResponse
            {
                Relationship = response
            };
        }
    }
}
