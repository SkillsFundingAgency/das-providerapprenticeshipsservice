using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using NUnit.Framework;
using Moq;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests.MockClasses;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.UnitTests
{
    [TestFixture]
    public class FetchDocumentsFromFeed
    {
        private string latestFileName = "latest";

        private string urlToApi = "https://services-at.fct-sfa.com";

        [Test]
        public void FetchOneDocument()
        {
            var mockFeedProcessorClient = GetMockFeedProcessorClient();

            var reader = new ContractFeedReader(mockFeedProcessorClient.Object);
            var dataProvider = new ContractFeedProcessor(reader, new MockContractFeedEventValidator());
            var repository = new InMemoryProviderAgreementStatusRepository(Mock.Of<ILog>());

            var service = new ProviderAgreementStatusService(dataProvider, repository);

            service.UpdateProviderAgreementStatuses();

            var s = repository.GetContractEvents(10017407);
            var bookmark = repository.GetMostRecentBookmarkId();
        }

        private Mock<ContractFeedProcessorHttpClient> GetMockFeedProcessorClient()
        {
            var fakeResponseHandler = new FakeResponseHandler();

            for (int i = 0; i < 13; i++)
            {
                var endOfUrl = i == 0 ? "" : $"/{i}";
                fakeResponseHandler.AddFakeResponse(
                    new Uri($"{urlToApi}/api/contracts/notifications{endOfUrl}"), CreateTestData(i)
                );
            }

            var mockFeedProcessorClient = new Mock<ContractFeedProcessorHttpClient>();
            mockFeedProcessorClient.Setup(m => m.BaseAddress).Returns(urlToApi);
            mockFeedProcessorClient.Setup(m => m.GetAuthorizedHttpClient()).Returns(() => new HttpClient(fakeResponseHandler));
            return mockFeedProcessorClient;
        }

        private HttpResponseMessage CreateTestData(int fileNo)
        {
            var fileName = fileNo < 1 ? latestFileName : $"{fileNo:D3}";
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileContent = File.ReadAllText(dir + $"\\TestData\\{fileName}.xml");
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(fileContent) };
        }

        private class FakeResponseHandler : DelegatingHandler
        {
            private readonly Dictionary<Uri, HttpResponseMessage> _FakeResponses = new Dictionary<Uri, HttpResponseMessage>();

            public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
            {
                _FakeResponses.Add(uri, responseMessage);
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                if (_FakeResponses.ContainsKey(request.RequestUri))
                {
                    return _FakeResponses[request.RequestUri];
                }
                else
                {
                    return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
                }

            }
        }
    }
}
