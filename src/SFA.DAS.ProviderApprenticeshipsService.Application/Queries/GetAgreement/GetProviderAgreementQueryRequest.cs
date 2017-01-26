using MediatR;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement
{
    public class GetProviderAgreementQueryRequest : IAsyncRequest<GetProviderAgreementQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}