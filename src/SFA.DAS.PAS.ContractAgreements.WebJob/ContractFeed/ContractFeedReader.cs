﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;

using SFA.DAS.NLog.Logger;
using System.Diagnostics;
using System.Net.Http;
using Polly;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedReader
    {
        private readonly IContractFeedProcessorHttpClient _httpClient;
        private readonly ILog _logger;
        private const string MostRecentPageUrl = "/api/contracts/notifications";

        public ContractFeedReader(IContractFeedProcessorHttpClient httpClient, ILog logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public string LatestPageUrl => $"{_httpClient.BaseAddress}{MostRecentPageUrl}";

        public void Read(string pageUri, ReadDirection direction, Func<string, string, Navigation, bool> pageWriter)
        {
            var relationshipType = direction == ReadDirection.Forward ? "next-archive" : "prev-archive";
            var continueToNextPage = true;

            while (continueToNextPage && !string.IsNullOrEmpty(pageUri))
            {
                var response = CallEndpointAndReturnResultForFullUrl(pageUri);
                SyndicationFeed feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response.Content)));
                Navigation pageNavigation = GetPageNavigation(feed);
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

            const string NextRelationshipType = "next-archive";
            const string PreviousRelationshipType = "prev-archive";
            string previousLink = feed.Links.SingleOrDefault(li => li.RelationshipType == PreviousRelationshipType)?.Uri.ToString();
            string nextLink = feed.Links.SingleOrDefault(li => li.RelationshipType == NextRelationshipType)?.Uri.ToString();

            return new Navigation(previousLink, nextLink);
        }

        private T LogTiming<T>(string actionDescription, Func<T> func)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = func();
            stopwatch.Stop();
            _logger.Trace($"It took {stopwatch.ElapsedMilliseconds} milliseconds to {actionDescription}");

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
                    _logger.Info($"Retry {retryCount} for page {url}");
                })
                .Execute(() => LogTiming($"download feed page {url}", () => client.GetAsync(url).Result));

                if (content.StatusCode == HttpStatusCode.NotFound)
                    return new HttpResult(HttpStatusCode.NotFound, string.Empty);

                content.EnsureSuccessStatusCode();

                return new HttpResult(content.StatusCode, content.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                var aggregateException = ex as AggregateException;
                if (aggregateException != null)
                {
                    var aex = aggregateException;
                    foreach (var exception in aex.InnerExceptions)
                    {
                        _logger.Error(exception, $"Error in contact feed reader, calling endpoint {url}");
                    }
                    throw aex.InnerExceptions.First();
                }
                _logger.Error(ex, $"Error in contact feed reader, calling endpoint {url}");
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
}
