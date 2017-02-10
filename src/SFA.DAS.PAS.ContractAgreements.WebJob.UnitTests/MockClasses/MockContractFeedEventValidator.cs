using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses
{
    public class MockContractFeedEventValidator : IContractFeedEventValidator
    {

        public bool Validate(ContractFeedEvent contractFeedEvent)
        {
            return contractFeedEvent.HierarchyType.ToLower() == "contract";
        }
    }
}
