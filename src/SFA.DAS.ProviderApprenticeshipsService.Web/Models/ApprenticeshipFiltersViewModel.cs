using System.Collections.Generic;
using IdentityServer3.Core.Validation;
using Microsoft.ServiceBus.Management;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipFiltersViewModel
    {
        public ApprenticeshipFiltersViewModel()
        {
            Status = new List<string>();
            RecordStatus = new List<string>();
            Employer = new List<string>();
            Course = new List<string>();

            ApprenticeshipStatusOptions = new List<KeyValuePair<string, string>>();
            TrainingCourseOptions = new List<KeyValuePair<string, string>>();
            RecordStatusOptions = new List<KeyValuePair<string, string>>();
            EmployerOrganisationOptions = new List<KeyValuePair<string, string>>();
        }

        //options available
        public List<KeyValuePair<string, string>> ApprenticeshipStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> TrainingCourseOptions { get; set; }
        public List<KeyValuePair<string, string>> RecordStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> EmployerOrganisationOptions { get; set; }

        //options selected
        
        public List<string> Status { get; set; }
        public List<string> RecordStatus { get; set; }
        public List<string> Employer { get; set; }
        public List<string> Course { get; set; }
    }
}