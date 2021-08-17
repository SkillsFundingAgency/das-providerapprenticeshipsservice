using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class AgreementOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly IAgreementMapper _agreementMapper;       
        public AgreementOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger,
            IAgreementMapper agreementMapper)
        : base(mediator, hashingService, logger)
        {
            _agreementMapper = agreementMapper;            
        }

        public async Task<AgreementsViewModel> GetAgreementsViewModel(long providerId, string organisation)
        {
            Logger.Info($"Getting agreements for provider: {providerId}", providerId);

            return new AgreementsViewModel
            {
                CommitmentAgreements = await GetCommitmentAgreements(providerId, organisation),
                SearchText = organisation
            };
        }      

        private async Task<IEnumerable<CommitmentAgreement>> GetCommitmentAgreements(long providerId, string searchTerm)
        {
            var response = await Mediator.Send(new GetCommitmentAgreementsQueryRequest
            {
                ProviderId = providerId
            });

            var result = response.CommitmentAgreements.Select(_agreementMapper.Map);

            return result
                    .Where(v => string.IsNullOrWhiteSpace(searchTerm.ToLower())
                        || (v.OrganisationName.ToLower().Contains(searchTerm.ToLower()))
                        || (string.IsNullOrWhiteSpace(v.OrganisationName) == false && v.OrganisationName.ToLower().Contains(searchTerm.ToLower())))
                    .OrderBy(v => v.OrganisationName)
                    .ThenBy(ca => ca.AgreementID)
                    .GroupBy(m => new { m.OrganisationName, m.AgreementID })
                    .Select(x => x.FirstOrDefault())
                    .ToList();
        }
    }
}