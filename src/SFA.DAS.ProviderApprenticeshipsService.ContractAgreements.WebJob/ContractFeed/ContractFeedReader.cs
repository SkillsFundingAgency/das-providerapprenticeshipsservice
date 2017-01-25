using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;

using SFA.DAS.NLog.Logger;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedReader
    {
        private readonly IContractFeedProcessorHttpClient _httpClient;

        private readonly ILog _logger;

        public ContractFeedReader(IContractFeedProcessorHttpClient httpClient, ILog logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        private const string MostRecentPageUrl = "/api/contracts/notifications";

        private const string VendorAtomMediaType = "application/vnd.sfa.contract.v1+atom+xml";

        public void Read(int pageNumber, Action<int, string> pageWriter)
        {
            var url = $"{_httpClient.BaseAddress}{MostRecentPageUrl}/{pageNumber}";
            var response = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, url);
            SyndicationFeed feed;
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response.Content)));
                pageWriter(ExtractPageNumberFromFeedItem(feed), response.Content);
            }
            else
            {
                var urlLatest = $"{_httpClient.BaseAddress}{MostRecentPageUrl}";
                var responseLatest = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, urlLatest);
                feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(responseLatest.Content)));
                pageWriter(ExtractPageNumberFromFeedItem(feed), responseLatest.Content);
            }

            SyndicationLink link;

            do
            {
                link = feed?.Links.FirstOrDefault(li => li.RelationshipType == "next-archive");

                if (link != null)
                {
                    response = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, link.Uri.ToString());
                    feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response.Content)));
                    pageWriter(ExtractPageNumberFromFeedItem(feed), response.Content);
                }
            } while (link != null);
        }

        private static int ExtractPageNumberFromFeedItem(SyndicationFeed feed)
        {
            int pageNumber;

            if (feed.Links.All(li => li.RelationshipType != "next-archive"))
            {
                pageNumber = 0;
            }
            else if (feed.Links.All(li => li.RelationshipType != "prev-archive"))
            {
                pageNumber = 1;
            }
            else
            {
                var previousPageNumber = ExtractPageNumberFromFeedLink(feed.Links.Single(li => li.RelationshipType == "prev-archive"));
                pageNumber = previousPageNumber + 1;
            }

            return pageNumber;
        }

        private static int ExtractPageNumberFromFeedLink(SyndicationLink link)
        {
            var linkUri = link.Uri.AbsoluteUri;
            var pageNumber = int.Parse(linkUri.Substring(linkUri.LastIndexOf("/", StringComparison.Ordinal) + 1));
            return pageNumber;
        }

        private HttpResult CallEndpointAndReturnResultForFullUrl(string mediaType, string url)
        {
            using (var client = _httpClient.GetAuthorizedHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                try
                {
                    var content = client.GetAsync(url).Result;
                    if (content.StatusCode == HttpStatusCode.NotFound) return new HttpResult(HttpStatusCode.NotFound, string.Empty);
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
