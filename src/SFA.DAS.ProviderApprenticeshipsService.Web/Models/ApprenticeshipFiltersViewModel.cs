using System.Collections.Generic;
using IdentityServer3.Core.Validation;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipFiltersViewModel
    {
        public ApprenticeshipFiltersViewModel()
        {
            ApprenticeshipStatuses = new List<string>();
            RecordStatuses = new List<string>();
            TrainingProviders = new List<string>();
            EmployerOrganisations = new List<string>();
            TrainingCourses = new List<string>();

            TrainingProvidersOptions = new List<KeyValuePair<string, string>>();
            ApprenticeshipStatusOptions = new List<KeyValuePair<string, string>>();
            TrainingCourseOptions = new List<KeyValuePair<string, string>>();
            RecordStatusOptions = new List<KeyValuePair<string, string>>();
            EmployerOrganisationOptions = new List<KeyValuePair<string, string>>();
        }

        //options available
        public List<KeyValuePair<string, string>> TrainingProvidersOptions { get; set; }
        public List<KeyValuePair<string, string>> ApprenticeshipStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> TrainingCourseOptions { get; set; }
        public List<KeyValuePair<string, string>> RecordStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> EmployerOrganisationOptions { get; set; }

        //options selected
        public List<string> ApprenticeshipStatuses { get; set; }
        public List<string> RecordStatuses { get; set; }
        public List<string> TrainingProviders { get; set; }
        public List<string> EmployerOrganisations { get; set; }
        public List<string> TrainingCourses { get; set; }
    }
}