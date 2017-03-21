using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IProviderAgreementStatusRepository
    {
        Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId);

        Task<Guid?> GetLatestBookmark();

        Task AddContractEventsForPage(List<ContractFeedEvent> contractFeedEvents, Guid newBookmark);

        Task<int> GetCountOfContracts();
    }
}