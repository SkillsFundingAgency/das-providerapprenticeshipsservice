using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Learners.Validators;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.AcademicYear;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Validation
{
    public class ApprovedApprenticeshipValidator : ApprenticeshipCoreValidator, IApprovedApprenticeshipValidator
    {
        private readonly IAcademicYearValidator _academicYearValidator;

        public ApprovedApprenticeshipValidator(IApprenticeshipValidationErrorText validationText,
            ICurrentDateTime currentDateTime,
            IAcademicYearDateProvider academicYear,
            IAcademicYearValidator academicYearValidator,
            IUlnValidator ulnValidator)
            : base(validationText, currentDateTime, academicYear, ulnValidator)
        {
            _academicYearValidator = academicYearValidator;
        }

        public Dictionary<string, string> ValidateToDictionary(ApprenticeshipViewModel model)
        {
            var result = Validate(model);

            return result.Errors.ToDictionary(
                approvedError => approvedError.PropertyName, approvedError => approvedError.ErrorMessage);
        }

        public Dictionary<string, string> ValidateAcademicYear(CreateApprenticeshipUpdateViewModel model)
        {
            var dict = new Dictionary<string, string>();

            if (model.StartDate?.DateTime != null && 
                _academicYearValidator.Validate(model.StartDate.DateTime.Value) == AcademicYearValidationResult.NotWithinFundingPeriod)
            {
                dict.Add($"{nameof(model.StartDate)}", ValidationText.AcademicYearStartDate01.Text);
            }

            return dict;
        }

        public Dictionary<string, string> ValidateApprovedEndDate(CreateApprenticeshipUpdateViewModel updatedApprenticeship)
        {
            var dict = new Dictionary<string, string>();

            if (updatedApprenticeship.EndDate != null)
            {
                if (updatedApprenticeship.OriginalApprenticeship.HasHadDataLockSuccess)
                {
                    //todo: helper for year and month only
                    var now = CurrentDateTime.Now;
                    if (new DateTime(updatedApprenticeship.EndDate.Year.Value, updatedApprenticeship.EndDate.Month.Value, 1) > new DateTime(now.Year, now.Month, 1))
                        dict.Add($"{nameof(updatedApprenticeship.EndDate)}", ValidationText.EndDateBeforeOrIsCurrentMonth.Text);
                }
            }

            return dict;
        }
    }
}