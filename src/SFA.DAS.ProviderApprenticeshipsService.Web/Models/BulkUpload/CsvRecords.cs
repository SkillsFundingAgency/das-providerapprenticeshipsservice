namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    internal class CsvRecords
    {
        public string GivenName{ get; set; }

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

        // What is this?
        // ToDo: InUse?
        public string EPAPrice  { get; set; }

        // ToDo: InUse?
        public string EPAOrgId { get; set; }  // Validate Startwith EPA...

        // ToDo: Do we set Emplyer ref here?
        public string EmpRef { get; set; }

        public string ProvRef { get; set; }

        public int? ULN { get; set; }
    }
}