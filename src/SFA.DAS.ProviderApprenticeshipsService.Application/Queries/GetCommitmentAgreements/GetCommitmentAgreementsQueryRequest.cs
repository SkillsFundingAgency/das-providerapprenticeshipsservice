using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements
{
    public class GetCommitmentAgreementsQueryRequest : IAsyncRequest<GetCommitmentAgreementsQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}
