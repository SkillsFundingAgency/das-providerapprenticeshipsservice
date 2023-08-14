using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;

public class GetCommitmentAgreementsQueryRequest : IRequest<GetCommitmentAgreementsQueryResponse>
{
    public long ProviderId { get; set; }
}