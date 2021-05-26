using CsvHelper.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public sealed class CsvRecordMap : ClassMap<CsvRecord>
    {
        public CsvRecordMap()
        {
            // AutoMap();
            Map(x => x.CohortRef).Optional();
            Map(x => x.ULN);
            Map(x => x.FamilyName);
            Map(x => x.GivenNames);
            Map(x => x.DateOfBirth);
            Map(x => x.ProgType).Optional();
            Map(x => x.FworkCode).Optional();
            Map(x => x.PwayCode).Optional();
            Map(x => x.StdCode);
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.TotalPrice);
            Map(x => x.EPAOrgID);
            Map(x => x.ProviderRef);
            Map(x => x.EmployerRef).Optional();
            Map(x => x.EmailAddress).Optional();
            Map(x => x.OffTheJobTrainingHours).Optional();
        }
    }
}