using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipFiltersViewModel
    {
        //options available
        public KeyValuePair<string, string>[] TrainingProvidersOptions { get; set; }
        public KeyValuePair<int, string>[] ApprenticeshipStatusOptions { get; set; }

        //options selected
        public int[] ApprenticeshipStatuses { get; set; }
        public string[] RecordStatuses { get; set; }
        public string[] TrainingProviders { get; set; }
        public string[] EmployerOrganisations { get; set; }
        public string[] TrainingCourses { get; set; }
    }
}