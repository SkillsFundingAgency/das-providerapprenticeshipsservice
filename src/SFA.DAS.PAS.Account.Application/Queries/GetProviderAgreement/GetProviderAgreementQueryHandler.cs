using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

namespace SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement
{
    public class GetProviderAgreementQueryHandler : IRequestHandler<GetProviderAgreementQueryRequest, GetProviderAgreementQueryResponse>
    {
        private readonly IAgreementStatusQueryRepository _agreementStatusQueryRepository;
        private readonly IProviderAgreementStatusConfiguration _configuration;

        public GetProviderAgreementQueryHandler(IAgreementStatusQueryRepository agreementStatusQueryRepository, IProviderAgreementStatusConfiguration configuration)
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
