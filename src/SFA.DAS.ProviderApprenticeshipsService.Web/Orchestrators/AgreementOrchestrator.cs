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
    public interface IAgreementOrchestrator
    {
        Task<AgreementsViewModel> GetAgreementsViewModel(long providerId, string organisation);
    }

    public sealed class AgreementOrchestrator : IAgreementOrchestrator
    {
        private readonly IAgreementMapper _agreementMapper;
        private readonly IMediator _mediator;
        private readonly IProviderCommitmentsLogger _logger;

        public AgreementOrchestrator(
            IMediator mediator,
            IProviderCommitmentsLogger logger,
            IAgreementMapper agreementMapper)
        {
            _agreementMapper = agreementMapper;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<AgreementsViewModel> GetAgreementsViewModel(long providerId, string organisation)
        {
            _logger.Info($"Getting agreements for provider: {providerId}", providerId);

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
            var response = await _mediator.Send(new GetCommitmentAgreementsQueryRequest
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