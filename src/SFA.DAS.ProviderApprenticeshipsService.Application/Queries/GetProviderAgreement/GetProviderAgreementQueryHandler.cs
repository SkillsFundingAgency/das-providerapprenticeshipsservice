using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderAgreement
{
    public class GetProviderAgreementQueryHandler : IRequestHandler<GetProviderAgreementQueryRequest, GetProviderAgreementQueryResponse>
    {
        private readonly IAgreementStatusQueryRepository _agreementStatusQueryRepository;
        private readonly ProviderApprenticeshipsServiceConfiguration _configuration;

        public GetProviderAgreementQueryHandler(IAgreementStatusQueryRepository agreementStatusQueryRepository, ProviderApprenticeshipsServiceConfiguration configuration)
        {
            _agreementStatusQueryRepository = agreementStatusQueryRepository;
            _configuration = configuration;
        }

        public async Task<GetProviderAgreementQueryResponse> Handle(GetProviderAgreementQueryRequest message, CancellationToken cancellationToken)
        {
            if(!_configuration.CheckForContractAgreements)
                return new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed };

            var res = await _agreementStatusQueryRepository.GetContractEvents(message.ProviderId);
            var providerAgreementStatus = res.Any(m => m.Status.Equals("approved", StringComparison.CurrentCultureIgnoreCase))
                        ? ProviderAgreementStatus.Agreed : ProviderAgreementStatus.NotAgreed;
            
            return new GetProviderAgreementQueryResponse { HasAgreement = providerAgreementStatus };
        }
    }
}
