using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractFeedEventValidator
    {
        bool Validate(ContractFeedEvent contractFeedEvent);
    }
}