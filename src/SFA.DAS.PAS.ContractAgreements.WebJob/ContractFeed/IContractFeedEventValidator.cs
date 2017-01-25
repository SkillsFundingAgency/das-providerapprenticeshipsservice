using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractFeedEventValidator
    {
        bool Validate(ContractFeedEvent contractFeedEvent);
    }
}