using System;
using System.IO;
using System.Linq;
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

        public void Read(Func<int, string, bool> pageWriter)
        {
            GetLatestItemsThenWalkLinksToTheStart(pageWriter);
        }

        private void GetLatestItemsThenWalkLinksToTheStart(Func<int, string, bool> pageWriter)
        {
            var response = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, _httpClient.BaseAddress + MostRecentPageUrl);
            var feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response)));
            var shouldContinue = pageWriter(ExtractPageNumberFromFeedItem(feed), response);

            SyndicationLink link;

            do
            {
                link = feed?.Links.FirstOrDefault(li => li.RelationshipType == "prev-archive");

                if (link != null && shouldContinue)
                {
                    response = CallEndpointAndReturnResultForFullUrl(VendorAtomMediaType, link.Uri.ToString());
                    feed = SyndicationFeed.Load(new XmlTextReader(new StringReader(response)));
                    shouldContinue = pageWriter(ExtractPageNumberFromFeedItem(feed), response);
                }
            } while (link != null && shouldContinue);
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

        private string CallEndpointAndReturnResultForFullUrl(string mediaType, string url)
        {
            using (var client = _httpClient.GetAuthorizedHttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

                try
                {
                    var content = client.GetAsync(url).Result;
                    content.EnsureSuccessStatusCode();

                    return content.Content.ReadAsStringAsync().Result;
                }
                catch (AggregateException aex)
                {
                    foreach (var exception in aex.InnerExceptions)
                    {
                        _logger.Error(exception, $"Error in contact feed reader, calling endpoint {url}");
                    }
                    throw aex.InnerExceptions.First();
                }
            }
        }
    }
}
