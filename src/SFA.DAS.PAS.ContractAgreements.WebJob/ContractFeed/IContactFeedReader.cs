using System;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public interface IContractFeedReader
    {
        string LatestPageUrl { get; }
        void Read(string pageUri, ReadDirection direction, Func<string, string, Navigation, bool> pageWriter);
    }
}
