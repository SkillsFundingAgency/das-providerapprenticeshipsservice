﻿using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Enums;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.Account.Application.Queries.GetProviderAgreement
{
    public class GetProviderAgreementQueryHandler : IRequestHandler<GetProviderAgreementQueryRequest, GetProviderAgreementQueryResponse>
    {
        private readonly IProviderAgreementStatusRepository _providerAgreementStatusRepository;
        private readonly IProviderAgreementStatusConfiguration _configuration;

        public GetProviderAgreementQueryHandler(IProviderAgreementStatusRepository providerAgreementStatusRepository, IProviderAgreementStatusConfiguration configuration)
        {
            _providerAgreementStatusRepository = providerAgreementStatusRepository;
            _configuration = configuration;
        }

        public async Task<GetProviderAgreementQueryResponse> Handle(GetProviderAgreementQueryRequest message, CancellationToken cancellationToken)
        {
            if(!_configuration.CheckForContractAgreements)
                return new GetProviderAgreementQueryResponse { HasAgreement = ProviderAgreementStatus.Agreed };

            var res = await _providerAgreementStatusRepository.GetContractEvents(message.ProviderId);
            var providerAgreementStatus = res.Any(m => m.Status.Equals("approved", StringComparison.CurrentCultureIgnoreCase))
                        ? ProviderAgreementStatus.Agreed : ProviderAgreementStatus.NotAgreed;
            
            return new GetProviderAgreementQueryResponse { HasAgreement = providerAgreementStatus };
        }
    }
}
