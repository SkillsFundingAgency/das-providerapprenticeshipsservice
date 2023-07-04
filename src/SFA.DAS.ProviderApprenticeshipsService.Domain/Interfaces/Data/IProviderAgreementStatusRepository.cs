using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

public interface IProviderAgreementStatusRepository
{
    Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId);

    Task<Guid?> GetLatestBookmark();

    Task AddContractEventsForPage(IList<ContractFeedEvent> contractFeedEvents, Guid newBookmark);

    Task<int> GetCountOfContracts();
}