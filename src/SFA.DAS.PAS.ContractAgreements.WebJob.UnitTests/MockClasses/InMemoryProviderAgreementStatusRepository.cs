﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses
{
    public sealed class InMemoryProviderAgreementStatusRepository : IProviderAgreementStatusRepository
    {
        private readonly ILog _logger;

        public readonly List<ContractFeedEvent> Data;
        public int LastFullPageRead;

        public InMemoryProviderAgreementStatusRepository(ILog logger)
        {
            _logger = logger;
            Data = new List<ContractFeedEvent>();
        }

        public Task<IEnumerable<ContractFeedEvent>> GetContractEvents(long providerId)
        {
            return Task.FromResult(Data.Where(e => e.ProviderId == providerId));
        }

        public Task<int> GetLatestBookmark()
        {
            return Task.FromResult(LastFullPageRead);
        }

        Task<Guid?> IProviderAgreementStatusRepository.GetLatestBookmark()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCountOfContracts()
        {
            throw new NotImplementedException();
        }

        public Task AddContractEventsForPage(IList<ContractFeedEvent> contractFeedEvents, Guid newBookmark)
        {
            throw new NotImplementedException();
        }
    }
}
