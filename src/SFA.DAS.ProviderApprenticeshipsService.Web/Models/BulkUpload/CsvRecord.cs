namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public class CsvRecord
    {
        public string CohortRef { get; set; }

        public string ULN { get; set; }

        public string FamilyName { get; set; }

        public string GivenNames { get; set; }

        public string DateOfBirth { get; set; }

        public string ProgType { get; set; }

        public string FworkCode { get; set; }

        public string PwayCode { get; set; }

        public string StdCode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string TotalPrice { get; set; }

        public string EPAOrgID { get; set; }  // ToDO: Validate Startwith EPA...

        public string ProviderRef { get; set; }
    }
}