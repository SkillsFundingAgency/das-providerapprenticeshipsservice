using MediatR;

namespace SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement
{
    public class GetProviderAgreementQueryRequest : IRequest<GetProviderAgreementQueryResponse>
    {
        public long ProviderId { get; set; }
    }
}