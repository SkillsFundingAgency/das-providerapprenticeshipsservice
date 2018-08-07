using System;
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

        public Task<AgreementsViewModel> GetAgreementsViewModel(long providerId)
        {
            Logger.Info($"Getting agreements for provider: {providerId}", providerId: providerId);

            throw new NotImplementedException();

            //var response = await Mediator.SendAsync(new GetCommitmentAgreementsQueryRequest
            //{
            //    ProviderId = providerId
            //});

            //return response..ToList();
        }
    }
}