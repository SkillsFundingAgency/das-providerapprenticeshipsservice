using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedEventValidator : AbstractValidator<ContractFeedEvent>
    {
        public ContractFeedEventValidator()
        {
            RuleFor(r => r.HierarchyType).Must(m => m.ToLower() == "contract");
            RuleFor(r => r.FundingTypeCode).Must(m => m.ToLower() == "levy");
            RuleFor(r => r.ParentStatus).Must(m => m.ToLower() == "approved");
            RuleFor(r => r.Status).Must(m => m.ToLower() == "approved");

        }
    }
}
