using System.Net.Http;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractFeedProcessorHttpClient
    {
        HttpClient GetAuthorizedHttpClient();

        string BaseAddress { get; }
    }
}