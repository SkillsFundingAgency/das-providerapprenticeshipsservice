using System;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.AcademicYear;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class AcademicYearValidator : IAcademicYearValidator
    {
        public readonly ICurrentDateTime CurrentDateTime;
        public readonly IAcademicYearDateProvider AcademicYear;

        public AcademicYearValidator(ICurrentDateTime currentDateTime, IAcademicYearDateProvider academicYear)
        {
            CurrentDateTime = currentDateTime;
            AcademicYear = academicYear;
        }

        public AcademicYearValidationResult Validate(DateTime trainingStartDate)
        {
           if (trainingStartDate < AcademicYear.CurrentAcademicYearStartDate &&
                CurrentDateTime.Now > AcademicYear.LastAcademicYearFundingPeriod)
            {
                return AcademicYearValidationResult.NotWithinFundingPeriod;
            }

            return AcademicYearValidationResult.Success;
        }

        public bool IsAfterLastAcademicYearFundingPeriod => CurrentDateTime.Now > AcademicYear.LastAcademicYearFundingPeriod;
    }
}
