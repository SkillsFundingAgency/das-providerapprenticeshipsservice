using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;
using SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests.MockClasses;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.UnitTests;

public class TestHelper
{
    public Mock<IContractFeedProcessorHttpClient> MockFeedProcessorClient;

    private readonly string _urlToApi;

    public TestHelper(string urlToApi)
    {
        _urlToApi = urlToApi;
    }

    public ProviderAgreementStatusService SetUpProviderAgreementStatusService(
        InMemoryProviderAgreementStatusRepository repository)
    {
        MockFeedProcessorClient = GetMockFeedProcessorClient();

        var reader = new ContractFeedReader(MockFeedProcessorClient.Object, Mock.Of<ILogger<ContractFeedReader>>());
        var dataProvider = new ContractFeedProcessor(reader, new MockContractFeedEventValidator(), Mock.Of<ILogger<ContractFeedProcessor>>());

        var service = new ProviderAgreementStatusService(dataProvider, repository, Mock.Of<ILogger<ProviderAgreementStatusService>>());

        return service;
    }

    private Mock<IContractFeedProcessorHttpClient> GetMockFeedProcessorClient()
    {
        var fakeResponseHandler = new FakeResponseHandler();

        for (var index = 0; index < 13; index++)
        {
            var endOfUrl = index == 0 ? "" : $"/{index}";
            fakeResponseHandler.AddFakeResponse(new Uri($"{_urlToApi}/api/contracts/notifications{endOfUrl}"), CreateTestData(index));
        }
            
        var mockFeedProcessorClient = new Mock<IContractFeedProcessorHttpClient>();
        mockFeedProcessorClient.Setup(m => m.BaseAddress).Returns(_urlToApi);
        mockFeedProcessorClient.Setup(m => m.GetAuthorizedHttpClient()).Returns(() => new HttpClient(fakeResponseHandler));
        
        return mockFeedProcessorClient;
    }

    private static HttpResponseMessage CreateTestData(int fileNo)
    {
        var fileName = fileNo < 1 ? "latest" : $"{fileNo:D3}";
        var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fileContent = File.ReadAllText(dir + $"\\TestData\\{fileName}.xml");
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(fileContent) };
    }

    private class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> _fakeResponses = new();

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            _fakeResponses.Add(uri, responseMessage);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_fakeResponses.ContainsKey(request.RequestUri))
            {
                return Task.FromResult(_fakeResponses[request.RequestUri]);
            }

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request });

        }
    }
}