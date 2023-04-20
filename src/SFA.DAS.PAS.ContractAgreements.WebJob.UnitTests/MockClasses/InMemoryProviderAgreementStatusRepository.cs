using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Data;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses;

public sealed class InMemoryProviderAgreementStatusRepository : IProviderAgreementStatusRepository
{
    private readonly ILogger<InMemoryProviderAgreementStatusRepository> _logger;

    public readonly List<ContractFeedEvent> Data;
    public Guid? LastBookmarkRead;

    public InMemoryProviderAgreementStatusRepository(ILogger<InMemoryProviderAgreementStatusRepository> logger)
    {
        _logger = logger;
        Data = new List<ContractFeedEvent>();
    }

    public Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId)
    {
        return Task.FromResult(Data.Where(e => e.ProviderId == providerId));
    }

    public Task<Guid?> GetLatestBookmark()
    {
        return Task.FromResult(LastBookmarkRead);
    }

    public Task<int> GetCountOfContracts()
    {
        return Task.FromResult(Data.Count);
    }

    public Task AddContractEventsForPage(IList<ContractFeedEvent> contractFeedEvents, Guid newBookmark)
    {
        Data.AddRange(contractFeedEvents);
        LastBookmarkRead = newBookmark;

        return Task.FromResult(new object());
    }
}