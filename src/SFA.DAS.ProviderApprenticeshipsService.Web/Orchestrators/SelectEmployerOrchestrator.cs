using System.Threading.Tasks;
using MediatR;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetProviderRelationshipsWithPermission;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.Types;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers;
using SFA.DAS.ProviderRelationships.Types.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class SelectEmployerOrchestrator : BaseCommitmentOrchestrator
    {
        private readonly ISelectEmployerMapper _selectEmployerMapper;

        public SelectEmployerOrchestrator(
            IMediator mediator,
            ISelectEmployerMapper selectEmployerMapper,
            IHashingService hashingService,
            IProviderCommitmentsLogger logger) : base(mediator, hashingService, logger)
        {
            _selectEmployerMapper = selectEmployerMapper;
        }

        public async Task<ChooseEmployerViewModel> GetChooseEmployerViewModel(long providerId, EmployerSelectionAction action)
        {
            Logger.Info($"Getting choose employer view model", providerId);

            var relationshipsWithPermission = await Mediator.Send(new GetProviderRelationshipsWithPermissionQueryRequest
            {
                ProviderId = providerId,
                Permission = Operation.CreateCohort
            });

            var result = _selectEmployerMapper.Map(relationshipsWithPermission.ProviderRelationships, action);

            return result;
        }
    }
}