namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public class CsvRecord
    {
        public string CohortRef { get; set; }

        public long? ULN { get; set; }

        public string GivenNames { get; set; }

        public string FamilyName { get; set; }

        public string DateOfBirth { get; set; }

        public string NINumber { get; set; }

        public int? FworkCode { get; set; }

        public int? PwayCode { get; set; }

        public int? ProgType { get; set; }

        public int? StdCode { get; set; }

        public string LearnStartDate { get; set; }

        public string LearnPlanEndDate { get; set; }

        public int? TrainingPrice { get; set; }

        public string EPAPrice  { get; set; }

        public string EPAOrgID { get; set; }  // ToDO: Validate Startwith EPA...

        public string ProvRef { get; set; }
    }
}