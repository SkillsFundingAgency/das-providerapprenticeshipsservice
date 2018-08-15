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

        public async Task<AgreementsViewModel> GetAgreementsViewModel(long providerId)
        {
            Logger.Info($"Getting agreements for provider: {providerId}", providerId: providerId);

            var response = await Mediator.SendAsync(new GetCommitmentAgreementsQueryRequest
            {
                ProviderId = providerId
            });

            return new AgreementsViewModel
            {
                //todo: check bread crumb
                CommitmentAgreements = response.CommitmentAgreements.Select(_agreementMapper.Map)
                    .OrderBy(ca => ca.OrganisationName)
                    .ThenBy(ca => ca.CohortID)
            };
        }
    }
}