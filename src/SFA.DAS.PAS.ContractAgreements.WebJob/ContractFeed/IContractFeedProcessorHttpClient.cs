using System.Net.Http;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;

public interface IContractFeedProcessorHttpClient
{
    HttpClient GetAuthorizedHttpClient();

    string BaseAddress { get; }
}