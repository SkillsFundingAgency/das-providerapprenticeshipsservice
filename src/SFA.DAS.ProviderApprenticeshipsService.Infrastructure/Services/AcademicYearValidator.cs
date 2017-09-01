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
        public readonly IAcademicYear _academicYear;

        public AcademicYearValidator(ICurrentDateTime currentDateTime, IAcademicYear academicYear)
        {
            _currentDateTime = currentDateTime;
            _academicYear = academicYear;
        }

        public AcademicYearValidationResult Validate(DateTime startDate)
        {
           if (startDate < _academicYear.CurrentAcademicYearStartDate &&
                _currentDateTime.Now > _academicYear.CurrentAcademicYearFundingPeriod)
            {
                return AcademicYearValidationResult.NotWithinFundingPeriod;
            }

            return AcademicYearValidationResult.Success;
        }
    }
}
