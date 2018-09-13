using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetCommitmentAgreements;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Agreement;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Formatters;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public sealed class AgreementOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly IAgreementMapper _agreementMapper;
        private readonly ICsvFormatter _csvFormatter;
        private readonly IExcelFormatter _excelFormatter;

        public AgreementOrchestrator(
            IMediator mediator,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger,
            IAgreementMapper agreementMapper,
            ICsvFormatter csvFormatter,
            IExcelFormatter excelFormatter)
        : base(mediator, hashingService, logger)
        {
            _agreementMapper = agreementMapper;
            _csvFormatter = csvFormatter;
            _excelFormatter = excelFormatter;
        }

        public async Task<AgreementsViewModel> GetAgreementsViewModel(long providerId)
        {
            Logger.Info($"Getting agreements for provider: {providerId}", providerId: providerId);

            return new AgreementsViewModel
            {
                CommitmentAgreements = await GetCommitmentAgreements(providerId)
            };
        }

        public async Task<byte[]> GetAgreementsAsCsv(long providerId)
        {
            var agreements = await GetCommitmentAgreements(providerId);
            return _csvFormatter.Format(agreements);
        }

        public async Task<byte[]> GetAgreementsAsExcel(long providerId)
        {
            var agreements = await GetCommitmentAgreements(providerId);
            return _excelFormatter.Format(agreements);
        }

        private async Task<IEnumerable<CommitmentAgreement>> GetCommitmentAgreements(long providerId)
        {
            var response = await Mediator.SendAsync(new GetCommitmentAgreementsQueryRequest
            {
                ProviderId = providerId
            });

            return response.CommitmentAgreements.Select(_agreementMapper.Map)
                .OrderBy(ca => ca.OrganisationName)
                .ThenBy(ca => ca.CohortID);
        }
    }
}