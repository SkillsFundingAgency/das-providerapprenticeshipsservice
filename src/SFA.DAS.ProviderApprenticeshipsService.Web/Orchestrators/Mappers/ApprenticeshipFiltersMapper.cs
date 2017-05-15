﻿using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class ApprenticeshipFiltersMapper : IApprenticeshipFiltersMapper
    {
        public ApprenticeshipSearchQuery MapToApprenticeshipSearchQuery(ApprenticeshipFiltersViewModel filters)
        {
            var result = new ApprenticeshipSearchQuery
            {
                TrainingProviders = filters.TrainingProviders?.ToList() ?? new List<string>(),

            };

            return result;
        }

        public ApprenticeshipFiltersViewModel Map(Facets facets)
        {
            var result = new ApprenticeshipFiltersViewModel();

            var trainingProviders = new List<KeyValuePair<string, string>>();
            foreach (var tp in facets.TrainingProviders)
            {
                //todo: only seems to be a single value in the source?
                trainingProviders.Add(new KeyValuePair<string, string>(tp.Data, tp.Data));

                if (tp.Selected)
                {
                    result.TrainingProviders.Add(tp.Data);
                }

            }

            var statuses = new List<KeyValuePair<string, string>>();
            foreach (var status in facets.ApprenticeshipStatuses)
            {
                statuses.Add(new KeyValuePair<string, string>(status.Data.ToString(), status.Data.ToString()));

                if (status.Selected)
                {
                    result.ApprenticeshipStatuses.Add(status.Data.ToString());
                }
            }

            var courses = new List<KeyValuePair<string, string>>();
            foreach (var course in facets.TrainingCourses)
            {
                courses.Add(new KeyValuePair<string, string>(course.Data.Id, course.Data.Name));

                if (course.Selected)
                {
                    result.TrainingCourses.Add(course.Data.Id);
                }
            }

            var employers = new List<KeyValuePair<string, string>>();
            foreach (var employer in facets.EmployerOrganisations)
            {
                employers.Add(new KeyValuePair<string, string>(employer.Data, employer.Data));

                if (employer.Selected)
                {
                    result.EmployerOrganisations.Add(employer.Data);
                }
            }

            var recordStatuses = new List<KeyValuePair<string, string>>();
            foreach (var recordStatus in facets.RecordStatuses)
            {
                recordStatuses.Add(new KeyValuePair<string, string>(recordStatus.Data.ToString(), recordStatus.Data.ToString()));

                if (recordStatus.Selected)
                {
                    result.RecordStatuses.Add(recordStatus.Data.ToString());
                }
            }

            result.TrainingProvidersOptions = trainingProviders;
            result.ApprenticeshipStatusOptions = statuses;
            result.TrainingCourseOptions = courses;
            result.EmployerOrganisationOptions = employers;
            result.RecordStatusOptions = recordStatuses;

            return result;
        }
    }
}