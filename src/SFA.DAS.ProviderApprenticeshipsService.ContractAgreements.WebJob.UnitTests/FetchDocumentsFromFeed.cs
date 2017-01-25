using System;
using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;
using Moq;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests.MockClasses;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests
{
    [TestFixture]
    public class FetchDocumentsFromFeed
    {


        private string urlToApi = "https://services-at.fct-sfa.com";

        [Test]
        public async Task FetchAllDocument()
        {
            // Call first time with latest is document 6
            var helper = new TestHelper("latest", urlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());
            var service = helper.SetUpProviderAgreementStatusService(repository);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(13));

            var bookmark = repository.GetMostRecentContractFeedEvent().Result;

            // Call a second time with latest
            var helper2 = new TestHelper("latest", urlToApi);
            var service2 = helper2.SetUpProviderAgreementStatusService(repository);

            await service2.UpdateProviderAgreementStatuses();

            helper2.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(2));

            var bookmark2 = repository.GetMostRecentContractFeedEvent().Result;

            bookmark2.Should().Be(bookmark);
        }

        [Test(Description = "When latest contract was not taken from last page start on that page again")]
        public async Task FetchDocumentStartingOnLatestContract()
        {
            var helper = new TestHelper("latest", urlToApi);
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
            var service = helper.SetUpProviderAgreementStatusService(repository);

            repository.GetMostRecentPageNumber().Result.Should().Be(11);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(11);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(3));
            repository.GetMostRecentPageNumber().Result.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(0);
        }

        [TestCase(6, 11, 3, Description = "When latest contract have page number that page should be parsed first")]
        [TestCase(11, 0, 2, Description = "When latest contract page number == 0 then page number should be taken from first contract with page number > 0")]
        public async Task FetchDocuments(int firstPn, int secondPn, int called)
        {
            var helper = new TestHelper("latest", urlToApi);
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
                    PageNumber = firstPn
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
                    PageNumber = secondPn
                });
            var service = helper.SetUpProviderAgreementStatusService(repository);

            repository.GetMostRecentPageNumber().Result.Should().Be(Math.Max(firstPn, secondPn));
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(secondPn);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(called));
            repository.GetMostRecentPageNumber().Result.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(0);
        }

        [Test(Description = "")]
        public async Task FetchDocuments()
        {
            var helper = new TestHelper("latest", urlToApi);
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
                    PageNumber = 12
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
                    PageNumber = 0
                });
            var service = helper.SetUpProviderAgreementStatusService(repository);
            var expectedLatestId = "985509f9-6da6-48d2-b0e1-90ad8337def9";

            repository.GetMostRecentPageNumber().Result.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(0);
            repository.GetMostRecentContractFeedEvent().Result.Id.ToString().Should().NotBe(expectedLatestId);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(2));
            repository.GetMostRecentPageNumber().Result.Should().Be(12);
            repository.GetMostRecentContractFeedEvent().Result.PageNumber.Should().Be(0);
            repository.GetMostRecentContractFeedEvent().Result.Id.ToString().Should().Be(expectedLatestId);
        }
    }
}
