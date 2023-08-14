using System;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Diagnostics;
using System.Net.Http;
using Polly;
using Microsoft.Extensions.Logging;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;

public class ContractFeedReader : IContractFeedReader
{
    private readonly IContractFeedProcessorHttpClient _httpClient;
    private readonly ILogger<ContractFeedReader> _logger;
    private const string MostRecentPageUrl = "/api/contracts/notifications";

    public ContractFeedReader(IContractFeedProcessorHttpClient httpClient, ILogger<ContractFeedReader> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        LatestPageUrl = $"{_httpClient.BaseAddress}{MostRecentPageUrl}";
    }

    public string LatestPageUrl { get; }

    public void Read(string pageUri, ReadDirection direction, Func<string, string, Navigation, bool> pageWriter)
    {
        var relationshipType = direction == ReadDirection.Forward ? "next-archive" : "prev-archive";
        var continueToNextPage = true;

        while (continueToNextPage && !string.IsNullOrEmpty(pageUri))
        {
            var response = CallEndpointAndReturnResultForFullUrl(pageUri);
            var feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response.Content)));
            var pageNavigation = GetPageNavigation(feed);
            continueToNextPage = pageWriter(pageUri, response.Content, pageNavigation);

            if (continueToNextPage)
            {
                var lastPageUri = pageUri;
                pageUri = feed?.Links.FirstOrDefault(li => li.RelationshipType == relationshipType)?.Uri.ToString();
                if (string.IsNullOrEmpty(pageUri)
                    && direction == ReadDirection.Forward
                    && LatestPageUrl != lastPageUri)
                {
                    pageUri = LatestPageUrl;
                }
            }
        }
    }

    private static Navigation GetPageNavigation(SyndicationFeed feed)
    {
        if (feed?.Links == null || feed.Links.Count == 0)
            return new Navigation(null, null);

        const string nextRelationshipType = "next-archive";
        const string previousRelationshipType = "prev-archive";
        
        var previousLink = feed.Links.SingleOrDefault(li => li.RelationshipType == previousRelationshipType)?.Uri.ToString();
        var nextLink = feed.Links.SingleOrDefault(li => li.RelationshipType == nextRelationshipType)?.Uri.ToString();

        return new Navigation(previousLink, nextLink);
    }

    private T LogTiming<T>(string actionDescription, Func<T> func)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = func();
        stopwatch.Stop();
        _logger.LogTrace($"It took {stopwatch.ElapsedMilliseconds} milliseconds to {actionDescription}");

        return result;
    }

    private HttpResult CallEndpointAndReturnResultForFullUrl(string url)
    {
        var client = _httpClient.GetAuthorizedHttpClient();

        try
        {
            var content = Policy
                .Handle<HttpRequestException>()
                .Retry(3, (exception, retryCount) =>
                {
                    _logger.LogInformation($"Retry {retryCount} for page {url}");
                })
                .Execute(() => LogTiming($"download feed page {url}", () => client.GetAsync(url).Result));

            if (content.StatusCode == HttpStatusCode.NotFound)
                return new HttpResult(HttpStatusCode.NotFound, string.Empty);

            content.EnsureSuccessStatusCode();

            return new HttpResult(content.StatusCode, content.Content.ReadAsStringAsync().Result);
        }
        catch (Exception ex)
        {
            if (ex is AggregateException aggregateException)
            {
                foreach (var exception in aggregateException.InnerExceptions)
                {
                    _logger.LogError(exception, $"Error in contact feed reader, calling endpoint {url}");
                }
                throw aggregateException.InnerExceptions.First();
            }
            _logger.LogError(ex, $"Error in contact feed reader, calling endpoint {url}");
            throw;
        }
    }

    private struct HttpResult
    {
        public HttpResult(HttpStatusCode statusCode, string content)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }
    }
}