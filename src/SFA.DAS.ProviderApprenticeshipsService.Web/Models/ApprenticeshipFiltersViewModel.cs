using Microsoft.Ajax.Utilities;
using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models
{
    public class ApprenticeshipFiltersViewModel
    {
        public ApprenticeshipFiltersViewModel()
        {
            PageNumber = 1;

            Status = new List<string>();
            RecordStatus = new List<string>();
            Employer = new List<string>();
            Course = new List<string>();
            FundingStatus = new List<string>();

            ApprenticeshipStatusOptions = new List<KeyValuePair<string, string>>();
            TrainingCourseOptions = new List<KeyValuePair<string, string>>();
            RecordStatusOptions = new List<KeyValuePair<string, string>>();
            EmployerOrganisationOptions = new List<KeyValuePair<string, string>>();
            FundingStatusOptions = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> ApprenticeshipStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> TrainingCourseOptions { get; set; }
        public List<KeyValuePair<string, string>> RecordStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> EmployerOrganisationOptions { get; set; }
        public List<KeyValuePair<string, string>> FundingStatusOptions { get; set; }

        public List<string> Status { get; set; }
        public List<string> RecordStatus { get; set; }
        public List<string> Employer { get; set; }
        public List<string> Course { get; set; }
        public List<string> FundingStatus { get; set; }

        public int PageNumber { get; set; }
        public string SearchInput { get; set; }
        public bool ResetFilter { get; set; }
        public bool CheckCookie { get; set; }

        public bool HasValues()
        {
            return Status.Count > 0
                   || RecordStatus.Count > 0
                   || Employer.Count > 0
                   || Course.Count > 0
                   || FundingStatus.Count > 0
                   || !SearchInput.IsNullOrWhiteSpace()
                   || PageNumber > 1;
        }
    }
}