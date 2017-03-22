using System;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using System.Collections.Generic;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests
{
    [TestFixture]
    public class FetchDocumentsFromFeed
    {
        private const string UrlToApi = "https://services-at.fct-sfa.com";

        [Test]
        public async Task FetchAllDocuments()
        {
            // Feed is set to have 13 pages
            var helper = new TestHelper(UrlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());
            var service = helper.SetUpProviderAgreementStatusService(repository);

            await service.UpdateProviderAgreementStatuses();
            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(26)); // reading 13 pages to find the start then back through
            repository.LastBookmarkRead.Should().Be(Guid.Parse("985509f9-6da6-48d2-b0e1-90ad8337def9"));
            repository.Data.Count.Should().Be(122); // All pages read 12 * 10 + 2(items on latest page)
        }

        [Test(Description = "Updates the last bookmark if a new item is read from the feed")]
        public async Task UpdatesBookmarkIfNewItemOnLatestPage()
        {
            var helper = new TestHelper(UrlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());

            await repository.AddContractEventsForPage(new List<ContractFeedEvent> { new ContractFeedEvent
                {
                    FundingTypeCode = "MAIN",
                    HierarchyType = "CONTRACT",
                    Id = Guid.Parse("75419D76-212B-47E2-B0B0-0B46C94120E7"),
                    ProviderId = 10017566,
                    Status = "Approved",
                    Updated = DateTime.Parse("1998-12-07"),
                    PageNumber = 6
                } }, Guid.Parse("b0f5f0ea-2315-444d-8fa0-4c4c5d6f0e3c"));

            var service = helper.SetUpProviderAgreementStatusService(repository);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(2));
            repository.GetLatestBookmark().Result.Should().Be(Guid.Parse("985509f9-6da6-48d2-b0e1-90ad8337def9"));
        }

        [Test(Description = "Updates the last bookmark to the latest bookmark when it is now on a full page and there are items on the latest page")]
        public async Task UpdatesBookmarkIfPageIsNowFull()
        {
            var helper = new TestHelper(UrlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());

            await repository.AddContractEventsForPage(new List<ContractFeedEvent> { new ContractFeedEvent
                {
                    FundingTypeCode = "MAIN",
                    HierarchyType = "CONTRACT",
                    Id = Guid.Parse("75419D76-212B-47E2-B0B0-0B46C94120E7"),
                    ProviderId = 10017566,
                    Status = "Approved",
                    Updated = DateTime.Parse("1998-12-07"),
                    PageNumber = 6
                } }, Guid.Parse("3bb5ad99-d528-4e77-af23-d7639f3dc86c"));

            var service = helper.SetUpProviderAgreementStatusService(repository);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(4));
            repository.GetLatestBookmark().Result.Should().Be(Guid.Parse("985509f9-6da6-48d2-b0e1-90ad8337def9"));
        }
    }
}
