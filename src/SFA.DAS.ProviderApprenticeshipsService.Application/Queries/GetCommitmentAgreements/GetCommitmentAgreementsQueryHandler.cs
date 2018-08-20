using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements
{
    /// <summary>
    /// For all the provider's commitments, returns a subset of each commitment related to its agreement
    /// </summary>
    public class GetCommitmentAgreementsQueryHandler : IAsyncRequestHandler<GetCommitmentAgreementsQueryRequest, GetCommitmentAgreementsQueryResponse>
    {
        private readonly IProviderCommitmentsApi _commitmentsApi;

        public GetCommitmentAgreementsQueryHandler(IProviderCommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
        }

        public async Task<GetCommitmentAgreementsQueryResponse> Handle(GetCommitmentAgreementsQueryRequest message)
        {
            var commitmentAgreements = await _commitmentsApi.GetCommitmentAgreements(message.ProviderId);

            return new GetCommitmentAgreementsQueryResponse
            {
                CommitmentAgreements = commitmentAgreements
            };
        }
    }
}