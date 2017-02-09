using System;
using System.Threading.Tasks;

using FluentAssertions;
using Moq;
using NUnit.Framework;

using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

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
            // Config is set to read 6 pages at a time
            var helper = new TestHelper(UrlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());
            var service = helper.SetUpProviderAgreementStatusService(repository);

            await service.UpdateProviderAgreementStatuses();
            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(6)); // reading 6 pages
            repository.LastFullPageRead.Should().Be(6);
            repository.Data.Count.Should().Be(60); // Each page has 10 contract * 6 pages

            await service.UpdateProviderAgreementStatuses();
            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(12)); // reading 6 more pages
            repository.LastFullPageRead.Should().Be(12);
            repository.Data.Count.Should().Be(120);

            await service.UpdateProviderAgreementStatuses();
            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(14)); // reading latest + 1 (with 404 if no new pages), and the latest
            repository.LastFullPageRead.Should().Be(12);
            repository.Data.Count.Should().Be(122); // Only 2 contract on the last page

            await service.UpdateProviderAgreementStatuses();
            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(16)); 
            repository.LastFullPageRead.Should().Be(12);
            repository.Data.Count.Should().Be(122);

            await service.UpdateProviderAgreementStatuses();
            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(18));
            repository.LastFullPageRead.Should().Be(12);
            repository.Data.Count.Should().Be(122);
        }

        [Test(Description = "Increments the last full page read if a new full page is found in the feed")]
        public async Task FetchDocumentStartingOnLatestContract()
        {
            var helper = new TestHelper(UrlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());
            await repository.AddContractEvent(
                new ContractFeedEvent
                {
                    FundingTypeCode = "MAIN",
                    HierarchyType = "CONTRACT",
                    Id = Guid.Parse("75419D76-212B-47E2-B0B0-0B46C94120E7"),
                    ProviderId = 10017566,
                    Status = "Approved",
                    Updated = DateTime.Parse("1998-12-07"),
                    PageNumber = 6
                });
            await repository.AddContractEvent(
                new ContractFeedEvent
                {
                    FundingTypeCode = "MAIN",
                    HierarchyType = "CONTRACT",
                    Id = Guid.Parse("3241322f-d600-4f84-b699-7a9964153ecd"),
                    ProviderId = 10017565,
                    Status = "Approved",
                    Updated = DateTime.Parse("1998-12-08"),
                    PageNumber = 11
                });
            repository.LastFullPageRead = 11;

            var service = helper.SetUpProviderAgreementStatusService(repository);

            repository.GetMostRecentPageNumber().Result.Should().Be(11);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(11);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(2));
            repository.GetMostRecentPageNumber().Result.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(13);
        }

        [Test(Description = "When no new full pages read but latest page has contracts")]
        public async Task FetchDocuments()
        {
            var helper = new TestHelper(UrlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());
            await repository.AddContractEvent(
                new ContractFeedEvent
                {
                    FundingTypeCode = "MAIN",
                    HierarchyType = "CONTRACT",
                    Id = Guid.Parse("75419D76-212B-47E2-B0B0-0B46C94120E7"),
                    ProviderId = 10017566,
                    Status = "Approved",
                    Updated = DateTime.Parse("1998-12-07"),
                    PageNumber = 1
                });
            await repository.AddContractEvent(
                new ContractFeedEvent
                {
                    FundingTypeCode = "MAIN",
                    HierarchyType = "CONTRACT",
                    Id = Guid.Parse("b0f5f0ea-2315-444d-8fa0-4c4c5d6f0e3c"),
                    ProviderId = 10017565,
                    Status = "Approved",
                    Updated = DateTime.Parse("1998-12-08"),
                    PageNumber = 12
                });
            repository.LastFullPageRead = 12;
            var service = helper.SetUpProviderAgreementStatusService(repository);
            var expectedLatestId = "985509f9-6da6-48d2-b0e1-90ad8337def9";

            repository.GetMostRecentPageNumber().Result.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.Id.ToString().Should().NotBe(expectedLatestId);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(2));
            repository.GetMostRecentPageNumber().Result.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(13);
            repository.GetMostRecentContractFeedEvent().Result.Id.ToString().Should().Be(expectedLatestId);
        }
    }
}
