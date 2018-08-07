using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement
{
    public class GetProviderAgreementQueryRequest : IAsyncRequest<GetProviderAgreementQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}