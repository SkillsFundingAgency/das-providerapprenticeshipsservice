using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;

public interface IContractFeedEventValidator
{
    bool Validate(ContractFeedEvent contractFeedEvent);
}