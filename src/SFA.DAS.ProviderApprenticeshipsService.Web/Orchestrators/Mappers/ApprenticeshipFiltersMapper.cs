using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Web.Helpers;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using ApprenticeshipStatus = SFA.DAS.ProviderApprenticeshipsService.Domain.ApprenticeshipStatus;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.Mappers
{
    public class ApprenticeshipFiltersMapper : IApprenticeshipFiltersMapper
    {
        public ApprenticeshipSearchQuery MapToApprenticeshipSearchQuery(ApprenticeshipFiltersViewModel filters)
        {
            var selectedEmployers = new List<long>();
            if (filters.Employer != null)
            {
                selectedEmployers.AddRange(filters.Employer.Select(long.Parse));
            }

            var result = new ApprenticeshipSearchQuery
            {
                EmployerOrganisationIds = selectedEmployers

            };

            return result;
        }

        public ApprenticeshipFiltersViewModel Map(Facets facets)
        {
            var result = new ApprenticeshipFiltersViewModel();

            var statuses = new List<KeyValuePair<string, string>>();
            foreach (var status in facets.ApprenticeshipStatuses)
            {
                var key = status.Data.ToString();
                var description = Enumerations.GetDescription((ApprenticeshipStatus) status.Data);

                statuses.Add(new KeyValuePair<string, string>(key, description));

                if (status.Selected)
                {
                    result.Status.Add(key);
                }
            }

            var courses = new List<KeyValuePair<string, string>>();
            foreach (var course in facets.TrainingCourses)
            {
                courses.Add(new KeyValuePair<string, string>(course.Data.Id, course.Data.Name));

                if (course.Selected)
                {
                    result.Course.Add(course.Data.Id);
                }
            }

            var employers = new List<KeyValuePair<string, string>>();
            foreach (var employer in facets.EmployerOrganisations)
            {
                employers.Add(new KeyValuePair<string, string>(employer.Data.Id.ToString(), employer.Data.Name));

                if (employer.Selected)
                {
                    result.Employer.Add(employer.Data.Id.ToString());
                }
            }

            var recordStatuses = new List<KeyValuePair<string, string>>();
            foreach (var recordStatus in facets.RecordStatuses)
            {
                var key = recordStatus.Data.ToString();
                var description = Enumerations.GetDescription((RecordStatus)recordStatus.Data);

                recordStatuses.Add(new KeyValuePair<string, string>(key, description));

                if (recordStatus.Selected)
                {
                    result.RecordStatus.Add(recordStatus.Data.ToString());
                }
            }

            result.ApprenticeshipStatusOptions = statuses;
            result.TrainingCourseOptions = courses;
            result.EmployerOrganisationOptions = employers;
            result.RecordStatusOptions = recordStatuses;

            return result;
        }
    }
}