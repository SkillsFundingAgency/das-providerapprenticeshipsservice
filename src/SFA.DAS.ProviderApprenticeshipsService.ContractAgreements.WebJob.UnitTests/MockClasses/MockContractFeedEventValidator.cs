using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests.MockClasses
{
    public class MockContractFeedEventValidator : AbstractValidator<ContractFeedEvent>
    {
        public MockContractFeedEventValidator()
        {
            RuleFor(r => r.HierarchyType).Must(m => m.ToLower() == "contract");
        }
    }
}
