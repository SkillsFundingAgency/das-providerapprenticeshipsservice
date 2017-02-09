using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

using Moq;

using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests
{
    public class TestHelper
    {
        public Mock<IContractFeedProcessorHttpClient> MockFeedProcessorClient;

        private readonly string _latestFileName;
        private readonly string _urlToApi;

        public TestHelper(string latestFileName, string urlToApi)
        {
            _latestFileName = latestFileName;
            _urlToApi = urlToApi;
        }

        public ProviderAgreementStatusService SetUpProviderAgreementStatusService(
            InMemoryProviderAgreementStatusRepository repository)
        {
            MockFeedProcessorClient = GetMockFeedProcessorClient();

            var reader = new ContractFeedReader(MockFeedProcessorClient.Object, Mock.Of<ILog>());
            var configuration = new ContractFeedConfiguration { ReadMaxPages = 6 };
            var dataProvider = new ContractFeedProcessor(reader, new MockContractFeedEventValidator(), configuration, Mock.Of<ILog>());

            var service = new ProviderAgreementStatusService(dataProvider, repository, Mock.Of<ILog>());

            return service;
        }

        private Mock<IContractFeedProcessorHttpClient> GetMockFeedProcessorClient()
        {
            var fakeResponseHandler = new FakeResponseHandler();

            for (int i = 0; i < 13; i++)
            {
                var endOfUrl = i == 0 ? "" : $"/{i}";
                fakeResponseHandler.AddFakeResponse(
                    new Uri($"{_urlToApi}/api/contracts/notifications{endOfUrl}"), CreateTestData(i)
                );
            }
            
            var mockFeedProcessorClient = new Mock<IContractFeedProcessorHttpClient>();
            mockFeedProcessorClient.Setup(m => m.BaseAddress).Returns(_urlToApi);
            mockFeedProcessorClient.Setup(m => m.GetAuthorizedHttpClient()).Returns(() => new HttpClient(fakeResponseHandler));
            return mockFeedProcessorClient;
        }

        private HttpResponseMessage CreateTestData(int fileNo)
        {
            var fileName = fileNo < 1 ? _latestFileName : $"{fileNo:D3}";
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

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                if (_FakeResponses.ContainsKey(request.RequestUri))
                {
                    return Task.FromResult(_FakeResponses[request.RequestUri]);
                }
                else
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request });
                }

            }
        }
    }
}