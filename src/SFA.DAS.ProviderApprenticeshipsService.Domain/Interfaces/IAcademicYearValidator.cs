using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IAcademicYearValidator
    {
        AcademicYearValidationResult Validate(DateTime trainingStartDate);

        bool IsAfterLastAcademicYearFundingPeriod { get; }
    }
}
