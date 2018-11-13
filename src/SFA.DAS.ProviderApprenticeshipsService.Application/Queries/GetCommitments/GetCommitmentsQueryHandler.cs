using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitments
{
    public class GetCommitmentsQueryHandler : IRequestHandler<GetCommitmentsQueryRequest, GetCommitmentsQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetCommitmentsQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetCommitmentsQueryResponse> Handle(GetCommitmentsQueryRequest message, CancellationToken cancellationToken)
        {
            var response = await _commitmentsApi.GetProviderCommitments(message.ProviderId);

            return new GetCommitmentsQueryResponse
            {
                Commitments = response
                    .Where(x => x.CommitmentStatus == CommitmentStatus.Active)
                    .ToList()
            };
        }
    }
}