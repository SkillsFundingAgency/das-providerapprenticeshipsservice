using System.Collections.Generic;
using System.Linq;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.ApprovedApprenticeshipValidation
{
    public class ApprovedApprenticeshipValidator : IApprovedApprenticeshipValidator
    {
        private readonly IApprenticeshipValidationErrorText _errorText;

        public ApprovedApprenticeshipValidator()
        {
            _errorText = new WebApprenticeshipValidationText();
        }

        public Dictionary<string, string> Validate(ApprenticeshipViewModel model)
        {
            var validator = new ApprovedApprenticeshipViewModelValidator(_errorText);
            var result = validator.Validate(model);

            return result.Errors.ToDictionary(
                approvedError => approvedError.PropertyName, approvedError => approvedError.ErrorMessage);
        }
    }
}