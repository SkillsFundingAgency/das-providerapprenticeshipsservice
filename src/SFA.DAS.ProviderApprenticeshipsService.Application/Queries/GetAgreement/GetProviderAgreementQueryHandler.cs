using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAgreement
{
    public class GetProviderAgreementQueryHandler : IAsyncRequestHandler<GetProviderAgreementQueryRequest, GetProviderAgreementQueryResponse>
    {
        private readonly IAgreementStatusQueryRepository _agreementStatusQueryRepository;

        public GetProviderAgreementQueryHandler(IAgreementStatusQueryRepository agreementStatusQueryRepository)
        {
            if (agreementStatusQueryRepository == null)
                throw new ArgumentNullException(nameof(agreementStatusQueryRepository));
            _agreementStatusQueryRepository = agreementStatusQueryRepository;
        }

        public async Task<GetProviderAgreementQueryResponse> Handle(GetProviderAgreementQueryRequest message)
        {
            var res = await _agreementStatusQueryRepository.GetContractEvents(message.ProviderId);

            var providerAgreementStatus = res.Any(m => m.Status.Equals("approved", StringComparison.CurrentCultureIgnoreCase))
                        ? ProviderAgreementStatus.Agreed : ProviderAgreementStatus.NotAgreed;

            return new GetProviderAgreementQueryResponse { HasAgreement = providerAgreementStatus };
        } 
    }
}
