using System.Threading.Tasks;

using FluentAssertions;

using NUnit.Framework;
using Moq;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests.MockClasses;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests
{
    [TestFixture]
    public class FetchDocumentsFromFeed
    {


        private string urlToApi = "https://services-at.fct-sfa.com";

        [Test]
        public async Task FetchOneDocument()
        {
            // Call first time with latest is document 6
            var helper = new TestHelper("latest06", urlToApi);
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());
            var service = helper.SetUpProviderAgreementStatusService(repository);

            await service.UpdateProviderAgreementStatuses();

            helper.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(6));

            var bookmark = repository.GetMostRecentBookmarkId().Result;

            // Call a second time with latest
            var helper2 = new TestHelper("latest", urlToApi);
            var service2 = helper2.SetUpProviderAgreementStatusService(repository);

            await service2.UpdateProviderAgreementStatuses();

            helper2.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(8));

            var bookmark2 = repository.GetMostRecentBookmarkId().Result;

            bookmark2.Should().NotBe(bookmark);

            // Call a third time with latest
            var helper3 = new TestHelper("latest", urlToApi);
            var service3 = helper3.SetUpProviderAgreementStatusService(repository);

            await service3.UpdateProviderAgreementStatuses();

            helper3.MockFeedProcessorClient.Verify(m => m.GetAuthorizedHttpClient(), Times.Exactly(1));

            var bookmark3 = repository.GetMostRecentBookmarkId().Result;

            bookmark.Should().NotBe(bookmark3);
            bookmark2.Should().Be(bookmark3);
        }
    }
}
