using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class AgreementOrchestrator : BaseCommitmentOrchestrator
    {
        public AgreementOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger)
        : base(mediator, hashingService, logger)
        {
        }

        public async Task<AgreementsViewModel> GetAgreementsViewModel(long providerId)
        {
            Logger.Info($"Getting agreements for provider: {providerId}", providerId: providerId);

            var response = await Mediator.SendAsync(new GetCommitmentAgreementsQueryRequest
            {
                ProviderId = providerId
            });

            return new AgreementsViewModel
            {
                //todo: move to mapper?
                CommitmentAgreements = response.CommitmentAgreements.Select(ca => new CommitmentAgreement
                {
                    // here we're basically mapping between what we call properties internally to what the view calls them
                    AgreementID = ca.AccountLegalEntityPublicHashedId,
                    CohortID = ca.Reference,
                    OrganisationName = ca.LegalEntityName
                })
            };
        }
    }
}