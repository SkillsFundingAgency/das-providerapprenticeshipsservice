using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.AcademicYear;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IAcademicYearValidator
    {
        AcademicYearValidationResult Validate(DateTime trainingStartDate);

        bool IsAfterLastAcademicYearFundingPeriod { get; }
    }
}
