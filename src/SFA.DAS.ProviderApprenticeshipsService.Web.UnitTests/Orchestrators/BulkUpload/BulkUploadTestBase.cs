using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    public class BulkUploadTestBase
    {
        protected static IEnumerable<string> GetInvalidColumnHeaders()
        {
            //yield return "ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId"; Cohortref optional so we dont need this test
            yield return "CohortRef,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,EndDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,TotalPrice,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,EPAOrgId,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,ProviderRef,EmailAddress,AgreementId";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,EmailAddress,AgreementId";
        }
    }
}