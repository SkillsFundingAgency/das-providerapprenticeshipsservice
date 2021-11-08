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

            var commitmentAgreements = await GetCommitmentAgreements(providerId);

            var filteredCommitmentAgreements = string.IsNullOrEmpty(organisation) 
                ? commitmentAgreements 
                : commitmentAgreements.Where(v => string.IsNullOrWhiteSpace(organisation.ToLower()) || (string.IsNullOrWhiteSpace(v.OrganisationName) == false && v.OrganisationName.ToLower().Replace(" ", String.Empty).Contains(organisation.ToLower().Replace(" ", String.Empty))));

            return new AgreementsViewModel
            {
                CommitmentAgreements = filteredCommitmentAgreements.ToList(),
                AllProviderOrganisationNames = commitmentAgreements.Select(ca => ca.OrganisationName).ToList(),
                SearchText = organisation
            };
        }      

        private async Task<IEnumerable<CommitmentAgreement>> GetCommitmentAgreements(long providerId)
        {
            var response = await Mediator.Send(new GetCommitmentAgreementsQueryRequest
            {
                ProviderId = providerId
            });

            var result = response.CommitmentAgreements.Select(_agreementMapper.Map);

            return result
                    .OrderBy(v => v.OrganisationName)
                    .ThenBy(ca => ca.AgreementID)
                    .GroupBy(m => new { m.OrganisationName, m.AgreementID })
                    .Select(x => x.FirstOrDefault())
                    .ToList();
        }
    }
}