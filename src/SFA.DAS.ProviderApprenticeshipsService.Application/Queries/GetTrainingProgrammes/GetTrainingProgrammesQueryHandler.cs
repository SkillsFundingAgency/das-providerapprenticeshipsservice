using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.GetTrainingProgrammes
{
    public sealed class GetTrainingProgrammesQueryHandler : IAsyncRequestHandler<GetTrainingProgrammesQueryRequest, GetTrainingProgrammesQueryResponse>
    {
        private readonly IApprenticeshipInfoService _apprenticeshipInfoService;

        public GetTrainingProgrammesQueryHandler(IApprenticeshipInfoService apprenticeshipInfoService)
        {
            _apprenticeshipInfoService = apprenticeshipInfoService;
        }

        public async Task<GetTrainingProgrammesQueryResponse> Handle(GetTrainingProgrammesQueryRequest message)
        {
            IEnumerable<ITrainingProgramme> programmes;
            var standardsTask = _apprenticeshipInfoService.GetStandardsAsync();
            if (!message.IncludeFrameworks)
            {
                programmes = (await standardsTask).Standards;
            }
            else
            {
                var getFrameworksTask = _apprenticeshipInfoService.GetFrameworksAsync();
                programmes = (await standardsTask).Standards.Union((await getFrameworksTask).Frameworks.Cast<ITrainingProgramme>());
            }

            return new GetTrainingProgrammesQueryResponse
            {
                TrainingProgrammes = programmes.OrderBy(m => m.Title).ToList()
            };
        }
    }
}
