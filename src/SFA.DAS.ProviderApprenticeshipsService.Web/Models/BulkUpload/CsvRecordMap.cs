using CsvHelper.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Models.BulkUpload
{
    public sealed class CsvRecordMap : ClassMap<CsvRecord>
    {
        public CsvRecordMap()
        {   
            Map(x => x.CohortRef).Optional();
            Map(x => x.ULN);
            Map(x => x.FamilyName);
            Map(x => x.GivenNames);
            Map(x => x.DateOfBirth);
            Map(x => x.StdCode);          
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.TotalPrice);
            Map(x => x.EPAOrgID);
            Map(x => x.ProviderRef);
            Map(x => x.AgreementId);
            Map(x => x.EmailAddress);
        }
    }

    public sealed class CsvRecordBlackListMap : ClassMap<CsvRecord>
    {
        public CsvRecordBlackListMap()
        {
            Map(x => x.CohortRef).Optional();
            Map(x => x.ULN);
            Map(x => x.FamilyName);
            Map(x => x.GivenNames);
            Map(x => x.DateOfBirth);
            Map(x => x.StdCode);
            Map(x => x.StartDate);
            Map(x => x.EndDate);
            Map(x => x.TotalPrice);
            Map(x => x.EPAOrgID);
            Map(x => x.ProviderRef);
            Map(x => x.AgreementId);
            Map(x => x.EmailAddress).Optional();
        }
    }
}