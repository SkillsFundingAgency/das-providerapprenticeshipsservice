using System.Threading.Tasks;
using MediatR;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.CreateCohort;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class CreateCohortOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly ICreateCohortMapper _createCohortMapper;

        public CreateCohortOrchestrator(
            IMediator mediator,
            ICreateCohortMapper createCohortMapper,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger) : base(mediator, hashingService, logger)
        {
            _createCohortMapper = createCohortMapper;
        }

        public async Task<CreateCohortViewModel> GetCreateCohortViewModel(long providerId)
        {
            Logger.Info($"Getting create cohort view model", providerId);

            var relationshipsWithPermission = await Mediator.Send(new GetProviderRelationshipsWithPermissionQueryRequest
            {
                ProviderId = providerId,
                Permission = Operation.CreateCohort
            });

            var result = _createCohortMapper.Map(relationshipsWithPermission.ProviderRelationships);

            return result;
        }
    }
}