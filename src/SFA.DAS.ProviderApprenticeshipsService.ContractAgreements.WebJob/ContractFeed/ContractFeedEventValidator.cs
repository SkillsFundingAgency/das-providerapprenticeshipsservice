using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedEventValidator : IContractFeedEventValidator
    {

        public bool Validate(ContractFeedEvent contractFeedEvent)
        {
            if (contractFeedEvent.HierarchyType.ToLower() != "contract")
                return false;
            if (contractFeedEvent.FundingTypeCode.ToLower() != "levy")
                return false;
            if (contractFeedEvent.ParentStatus.ToLower() != "approved")
                return false;
            if (contractFeedEvent.Status.ToLower() != "approved")
                return false;

            return true;
        }
    }
}
