using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services
{
    public class AcademicYearValidator : IAcademicYearValidator
    {

        public readonly ICurrentDateTime _currentDateTime;
        public readonly IAcademicYearDateProvider _academicYear;

        public AcademicYearValidator(ICurrentDateTime currentDateTime, IAcademicYearDateProvider academicYear)
        {
            _currentDateTime = currentDateTime;
            _academicYear = academicYear;
        }

        public AcademicYearValidationResult Validate(DateTime trainingStartDate)
        {
           if (trainingStartDate < _academicYear.CurrentAcademicYearStartDate &&
                _currentDateTime.Now > _academicYear.LastAcademicYearFundingPeriod)
            {
                return AcademicYearValidationResult.NotWithinFundingPeriod;
            }

            return AcademicYearValidationResult.Success;
        }
    }
}
