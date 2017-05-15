using System;
using System.Collections.Generic;
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
                TrainingProviders = new List<FacetItem<TrainingProvider>>
                {
                    new FacetItem<TrainingProvider> { Data = new TrainingProvider { Id = 1, Name = "TP1" } },
                    new FacetItem<TrainingProvider> { Data = new TrainingProvider { Id = 2, Name = "TP2" } },
                    new FacetItem<TrainingProvider> { Data = new TrainingProvider { Id = 3, Name = "TP3" } },
                    new FacetItem<TrainingProvider> { Data = new TrainingProvider { Id = 4, Name = "TP4" } },
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
                EmployerOrganisations = new List<FacetItem<EmployerOrganisation>>
                {
                    new FacetItem<EmployerOrganisation> { Data = new EmployerOrganisation { Id = 1, Name = "Employer Organisation 1"} },
                    new FacetItem<EmployerOrganisation> { Data = new EmployerOrganisation { Id = 2, Name = "Employer Organisation 2"} },
                    new FacetItem<EmployerOrganisation> { Data = new EmployerOrganisation { Id = 3, Name = "Employer Organisation 3 which has a long name"} },
                },
                RecordStatuses = new List<FacetItem<RecordStatus>>
                {
                    new FacetItem<RecordStatus> { Data = RecordStatus.ChangeRequested, Selected = true },
                    new FacetItem<RecordStatus> { Data = RecordStatus.ChangesForReview, Selected = false },
                    new FacetItem<RecordStatus> { Data = RecordStatus.ChangesPending, Selected = false },
                    new FacetItem<RecordStatus> { Data = RecordStatus.NoActionNeeded, Selected = false },
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

            return new ApprenticeshipSearchQueryResponse
            {
                Apprenticeships = all,
                Facets = facets
            };


            //var data = await _commitmentsApi.GetProviderApprenticeships(message.ProviderId, message.Query);

            //return new ApprenticeshipSearchQueryResponse
            //{
            //    Apprenticeships = data.Apprenticeships.ToList(),
            //    Facets = data.Facets
            //};

        }
    }
}
