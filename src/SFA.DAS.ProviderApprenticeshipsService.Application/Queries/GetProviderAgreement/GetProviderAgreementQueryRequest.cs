using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement
{
    public class GetProviderAgreementQueryRequest : IRequest<GetProviderAgreementQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}