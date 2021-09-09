using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements
{
    /// <summary>
    /// For all the provider's commitments, returns a subset of each commitment related to its agreement
    /// </summary>
    public class GetCommitmentAgreementsQueryHandler : IRequestHandler<GetCommitmentAgreementsQueryRequest, GetCommitmentAgreementsQueryResponse>
    {
        private readonly ICommitmentsV2ApiClient _commitmentsApi;

        public GetCommitmentAgreementsQueryHandler(ICommitmentsV2ApiClient commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetCommitmentAgreementsQueryResponse> Handle(GetCommitmentAgreementsQueryRequest message, CancellationToken cancellationToken)
        {
            var response = await _commitmentsApi.GetProviderCommitmentAgreement(message.ProviderId);

            return new GetCommitmentAgreementsQueryResponse
            {
                CommitmentAgreements = response.ProviderCommitmentAgreement
            };
        }
    }
}