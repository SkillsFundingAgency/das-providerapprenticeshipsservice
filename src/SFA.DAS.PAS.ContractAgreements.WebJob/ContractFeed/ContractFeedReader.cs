using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.ServiceModel.Syndication;
using System.Xml;

using SFA.DAS.NLog.Logger;
using System.Web;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public enum PageLinks
    {
        None = 0,
        Next = 1,
        Previous = 2,
        Both = Next & Previous
    }

    public sealed class Navigation
    {
        public Navigation(string previousPageUri, string nextPageUri)
        {
            PreviousPageUrl = previousPageUri;
            NextPageUrl = nextPageUri;
        }

        public string PreviousPageUrl { get; }
        public string NextPageUrl { get; }
        public bool IsStartPage => !string.IsNullOrWhiteSpace(NextPageUrl) && string.IsNullOrWhiteSpace(PreviousPageUrl);
    }

    public enum ReadDirection
    {
        Forward,
        Backward
    }

    public class ContractFeedReader
    {
        private readonly IContractFeedProcessorHttpClient _httpClient;
        private readonly ILog _logger;
        private const string MostRecentPageUrl = "/api/contracts/notifications";
        private const string VendorAtomMediaType = "application/vnd.sfa.contract.v1+atom+xml";

        public ContractFeedReader(IContractFeedProcessorHttpClient httpClient, ILog logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public string LatestPageUrl => $"{_httpClient.BaseAddress}{MostRecentPageUrl}";

        private static Navigation GetPageNavigation(SyndicationFeed feed)
        {
            if (feed == null || feed.Links == null || feed.Links.Count == 0)
                return new Navigation(null, null);

            const string NextRelationshipType = "next-archive";
            const string PreviousRelationshipType = "prev-archive";
            string previousLink = feed.Links.SingleOrDefault(li => li.RelationshipType == PreviousRelationshipType)?.Uri.ToString();
            string nextLink = feed.Links.SingleOrDefault(li => li.RelationshipType == NextRelationshipType)?.Uri.ToString();

            return new Navigation(previousLink, nextLink);
        }

        public void Read(string pageUri, ReadDirection direction, Func<string, string, Navigation, bool> pageWriter)
        {
            var relationshipType = direction == ReadDirection.Forward ? "next-archive" : "prev-archive";
            var continueToNextPage = true;
            
            while (continueToNextPage && !string.IsNullOrEmpty(pageUri))
            {
                var response = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, pageUri);
                SyndicationFeed feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response.Content)));
                Navigation pageNavigation = GetPageNavigation(feed);
                continueToNextPage = pageWriter(pageUri, response.Content, pageNavigation);

                if (continueToNextPage)
                    pageUri = feed?.Links.FirstOrDefault(li => li.RelationshipType == relationshipType)?.Uri.ToString();
            }
        }

        //public void ReadForward(string pageUri, Action<int, string, string, Navigation> pageWriter)
        //{
        //    var response = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, pageUri);
        //    SyndicationFeed feed;
        //    Navigation pageNavigation;

        //    feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response.Content)));
        //    pageNavigation = GetPageNavigation(feed);
        //    pageWriter(ExtractPageNumberFromFeedItem(feed), pageUri, response.Content, pageNavigation);

        //    SyndicationLink link;
        //    do
        //    {
        //        link = feed?.Links.FirstOrDefault(li => li.RelationshipType == "next-archive");
                
        //        if (link != null)
        //        {
        //            var newUrl = link.Uri.ToString();
        //            response = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, newUrl);
        //            feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response.Content)));
        //            pageNavigation = GetPageNavigation(feed);
        //            pageWriter(ExtractPageNumberFromFeedItem(feed), newUrl, response.Content, pageNavigation);
        //        }
        //    } while (link != null);
        //}

        //private static int ExtractPageNumberFromFeedItem(SyndicationFeed feed)
        //{
        //    int pageNumber;
        //    var pageNavigation = GetPageNavigation(feed);

        //    if (pageNavigation.IsStartPage)
        //    {
        //        pageNumber = 1; // First page
        //    }
        //    else
        //    {
        //        var previousPageNumber = ExtractPageNumberFromFeedLink(feed.Links.Single(li => li.RelationshipType == "prev-archive"));
        //        pageNumber = previousPageNumber + 1; // TODO: LWA - This is an incorrect assumption isn't it???
        //    }

        //    return pageNumber;
        //}

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
