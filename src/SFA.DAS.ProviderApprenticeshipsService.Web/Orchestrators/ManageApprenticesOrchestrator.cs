using System.Threading.Tasks;

using MediatR;

using SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetAllApprentices;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators
{
    public class ManageApprenticesOrchestrator
    {
        private readonly IMediator _mediator;

        public ManageApprenticesOrchestrator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ManageApprenticeshipsViewModel> GetApprenticeships(long providerId)
        {
            var data = await _mediator.SendAsync(new GetAllApprenticesRequest { ProviderId = providerId });
            
            return new ManageApprenticeshipsViewModel
                       {
                            Apprenticeships = data.Apprenticeships // ToDo: Map to view model
                       };
        }
    }
}