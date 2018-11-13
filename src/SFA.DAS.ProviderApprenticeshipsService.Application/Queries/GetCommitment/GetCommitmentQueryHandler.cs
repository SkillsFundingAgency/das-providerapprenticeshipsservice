using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitment
{
    public class GetCommitmentQueryHandler : IRequestHandler<GetCommitmentQueryRequest, GetCommitmentQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetCommitmentQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetCommitmentQueryResponse> Handle(GetCommitmentQueryRequest message, CancellationToken cancellationToken)
        {
            var commitment = await _commitmentsApi.GetProviderCommitment(message.ProviderId, message.CommitmentId);

            return new GetCommitmentQueryResponse
            {
                Commitment = commitment
            };
        }
    }
}