using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Queries.ApprenticeshipSearch
{
    public class ApprenticeshipSearchQueryHandler : IAsyncRequestHandler<ApprenticeshipSearchQueryRequest, ApprenticeshipSearchQueryResponse>
    {

        private readonly IProviderCommitmentsApi _commitmentsApi;

        public ApprenticeshipSearchQueryHandler(IProviderCommitmentsApi providerCommitmentsApi)
        {
            if (providerCommitmentsApi == null)
                throw new ArgumentNullException(nameof(providerCommitmentsApi));
            _commitmentsApi = providerCommitmentsApi;
        }

        public async Task<ApprenticeshipSearchQueryResponse> Handle(ApprenticeshipSearchQueryRequest message)
        {
            //temp! get all apps instead and mock out some filters
            var all = await _commitmentsApi.GetProviderApprenticeships(message.ProviderId);
            var facets = new Facets
            {
                TrainingProviders = new List<FacetItem<string>>
                {
                    new FacetItem<string> { Data = "TP1", Selected = false },
                    new FacetItem<string> { Data = "TP2", Selected = true },
                    new FacetItem<string> { Data = "TP3", Selected = true },
                    new FacetItem<string> { Data = "TP4", Selected = false }
                },
                ApprenticeshipStatuses = new List<FacetItem<ApprenticeshipStatus>>
                {
                    new FacetItem<ApprenticeshipStatus> { Data = ApprenticeshipStatus.WaitingToStart },
                    new FacetItem<ApprenticeshipStatus> { Data = ApprenticeshipStatus.Paused },
                    new FacetItem<ApprenticeshipStatus> { Data = ApprenticeshipStatus.Stopped, Selected = true},
                    new FacetItem<ApprenticeshipStatus> { Data = ApprenticeshipStatus.Paused },
                    new FacetItem<ApprenticeshipStatus> { Data = ApprenticeshipStatus.Finished },
                    new FacetItem<ApprenticeshipStatus> { Data = ApprenticeshipStatus.Live }
                },
                EmployerOrganisations = new List<FacetItem<string>>
                {
                    new FacetItem<string> { Data = "Emp1", Selected = false },
                    new FacetItem<string> { Data = "Emp2", Selected = true },
                    new FacetItem<string> { Data = "Emp3", Selected = false },
                },
                RecordStatuses = new List<FacetItem<RecordStatus>>
                {
                    new FacetItem<RecordStatus> { Data = RecordStatus.ChangeRequested, Selected = true },
                    new FacetItem<RecordStatus> { Data = RecordStatus.ChangesForReview, Selected = false },
                    new FacetItem<RecordStatus> { Data = RecordStatus.ChangesPending, Selected = false },
                    new FacetItem<RecordStatus> { Data = RecordStatus.NoActionNeeded, Selected = true },
                },
                TrainingCourses = new List<FacetItem<TrainingCourse>>
                {
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse { Id = "C1", Name = "Course 1", TrainingType = TrainingType.Framework },
                        Selected = true
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse { Id = "C2", Name = "Course 2", TrainingType = TrainingType.Framework }
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse { Id = "C3", Name = "Course 3", TrainingType = TrainingType.Standard },
                        Selected = true
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse { Id = "C4", Name = "Course 4", TrainingType = TrainingType.Framework }
                    },
                    new FacetItem<TrainingCourse>
                    {
                        Data = new TrainingCourse { Id = "C5", Name = "Course 5", TrainingType = TrainingType.Framework }
                    }
                }
            };

            //var data = await _commitmentsApi.GetProviderApprenticeships(message.ProviderId, message.Query);

            return new ApprenticeshipSearchQueryResponse
            {
                Apprenticeships = all,
                Facets = facets
            };

        }
    }
}
