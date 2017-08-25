using FluentValidation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class ValidationRegistry : Registry
    {
        public ValidationRegistry()
        {
            For<IValidator<ApprenticeshipViewModel>>().Use<ApprenticeshipViewModelValidator>();

            //AssemblyScanner.FindValidatorsInAssemblyContaining<ApprenticeshipViewModelValidator>()
            //    .ForEach(result => {
            //        For(result.InterfaceType)
            //           .Singleton()
            //           .Use(result.ValidatorType);
            //    });
        }
    }
}