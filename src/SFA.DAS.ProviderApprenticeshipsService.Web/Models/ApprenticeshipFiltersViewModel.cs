using System.Collections.Generic;

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