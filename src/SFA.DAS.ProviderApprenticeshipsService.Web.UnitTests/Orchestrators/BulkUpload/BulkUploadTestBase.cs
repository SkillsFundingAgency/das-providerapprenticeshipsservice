using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.UnitTests.Orchestrators.BulkUpload
{
    public class BulkUploadTestBase
    {
        protected static IEnumerable<string> GetInvalidColumnHeaders()
        {
            yield return "ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StartDate,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,EndDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,TotalPrice,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,EPAOrgId,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,ProviderRef";
            yield return "CohortRef,ULN,FamilyName,GivenNames,DateOfBirth,ProgType,FworkCode,PwayCode,StdCode,StartDate,EndDate,TotalPrice,EPAOrgId";
        }
    }
}