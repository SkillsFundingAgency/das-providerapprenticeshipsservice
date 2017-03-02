namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public class CsvRecord
    {
        public string CohortRef { get; set; }

        public long? ULN { get; set; }

        public string FamilyName { get; set; }

        public string GivenNames { get; set; }

        public string DateOfBirth { get; set; }

        public int? ProgType { get; set; }

        public int? FworkCode { get; set; }

        public int? PwayCode { get; set; }

        public int? StdCode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int? TotalPrice { get; set; }

        public string EPAOrgID { get; set; }  // ToDO: Validate Startwith EPA...

        public string ProviderRef { get; set; }
    }
}