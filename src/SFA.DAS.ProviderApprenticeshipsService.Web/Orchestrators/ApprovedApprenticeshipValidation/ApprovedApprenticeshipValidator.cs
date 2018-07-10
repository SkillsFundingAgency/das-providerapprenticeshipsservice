using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models.AcademicYear;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models.ApprenticeshipUpdate;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation
{
    public class ApprovedApprenticeshipValidator : IApprovedApprenticeshipValidator
    {
        private readonly IAcademicYearValidator _academicYearValidator;
        private readonly IApprenticeshipValidationErrorText _errorText;

        public ApprovedApprenticeshipValidator(IAcademicYearDateProvider academicYear, IAcademicYearValidator academicYearValidator)
        {
            _academicYearValidator = academicYearValidator;
            _errorText = new WebApprenticeshipValidationText(academicYear);
        }

        public Dictionary<string, string> Validate(ApprenticeshipViewModel model)
        {
            var validator = new ApprovedApprenticeshipViewModelValidator(_errorText);
            var result = validator.Validate(model);

            return result.Errors.ToDictionary(
                approvedError => approvedError.PropertyName, approvedError => approvedError.ErrorMessage);
        }

        public Dictionary<string, string> ValidateAcademicYear(CreateApprenticeshipUpdateViewModel model)
        {
            var dict = new Dictionary<string, string>();

            if (model.StartDate?.DateTime != null && 
                _academicYearValidator.Validate(model.StartDate.DateTime.Value) == AcademicYearValidationResult.NotWithinFundingPeriod)
            {
                dict.Add($"{nameof(model.StartDate)}", _errorText.AcademicYearStartDate01.Text);
            }

            return dict;
        }
    }
}