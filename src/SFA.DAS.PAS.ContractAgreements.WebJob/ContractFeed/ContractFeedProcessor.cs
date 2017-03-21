using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using SFA.DAS.NLog.Logger;
using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedProcessor : IContractDataProvider
    {
        private readonly XNamespace _nsAtom = "http://www.w3.org/2005/Atom";
        private readonly XNamespace _nsUrn = "urn:sfa:schemas:contract";

        private readonly ContractFeedReader _reader;

        private readonly IContractFeedEventValidator _validator;

        private readonly ContractFeedConfiguration _configuration;

        private readonly ILog _logger;

        public ContractFeedProcessor(
            ContractFeedReader reader, 
            IContractFeedEventValidator validator,
            ContractFeedConfiguration configuration,
            ILog logger)
        {
            _reader = reader;
            _validator = validator;
            _configuration = configuration;
            _logger = logger;
        }

        public string FindPageWithBookmark(Guid? latestBookmark)
        {
            _logger.Info($"Finding page for latest bookmark: {(latestBookmark.HasValue ? latestBookmark.ToString() : "[not set]")}");

            string currentPageUrl = _reader.LatestPageUrl;
            string startPageUrl = null;

            // Load page and check if contains bookmark
            _reader.Read(currentPageUrl, ReadDirection.Backward, (pageUri, pageContent, pageNavigation) =>
            {
                // Is this the first page?
                if (pageNavigation.IsStartPage)
                {
                    startPageUrl = pageUri;
                    _logger.Info($"Start page found at: {startPageUrl}");

                    return false;
                }

                // Look at items on the page for a match of book mark
                var doc = XDocument.Parse(pageContent);

                if (PageContainsBookmark(latestBookmark, doc))
                {
                    startPageUrl = pageUri;
                    _logger.Info($"Bookmark {latestBookmark.ToString()} found on page: {startPageUrl}");

                    return false;
                }

                return true;
            });

            return startPageUrl;
        }

        private bool PageContainsBookmark(Guid? latestBookmark, XDocument doc)
        {
            // process page in reverse order as items are provided in ascending datetime from fcs
            return doc.Descendants(_nsAtom + "entry")
                                    .Reverse()
                                    .Select(ExtractContractFeedEvent)
                                    .Any(x => x.Id == latestBookmark);
        }

        public int ReadEvents(string pageToReadUri, Guid? latestBookmark, Action<IEnumerable<ContractFeedEvent>, Guid?> saveRecordsAction)
        {
            _logger.Info($"Reading Events");

            var contractCount = 0;

            _reader.Read(pageToReadUri, ReadDirection.Forward, (pageUri, pageContent, pageNaviation) =>
            {
                var doc = XDocument.Parse(pageContent);

                // process page in reverse order as items are provided in ascending datetime from fcs
                var entries = doc.Descendants(_nsAtom + "entry")
                    .Reverse()
                    .Select(ExtractContractFeedEvent)
                    .ToList();

                var newBookmark = entries.FirstOrDefault()?.Id;
                 
                var matchingContracts = entries
                    .Where(contractFeedEvent => contractFeedEvent != null)
                    .TakeWhile(contractFeedEvent => latestBookmark == null || contractFeedEvent.Id != latestBookmark)
                    .Where(_validator.Validate)
                    .ToList();

                _logger.Info($"Adding: {matchingContracts.Count} from page: {pageUri}");

                saveRecordsAction(matchingContracts, newBookmark);

                contractCount += matchingContracts.Count;

                return true; // Continue reading more
            });

            return contractCount;
        }

        private ContractFeedEvent ExtractContractFeedEvent(XContainer element)
        {
            try
            {
                var id = element.Element(_nsAtom + "id")?.Value.Split(':').ElementAt(1);

                var contract = element
                    .Element(_nsAtom + "content")?
                    .Descendants(_nsUrn + "contract")?.First()
                    .Element(_nsUrn + "contracts")?.Descendants(_nsUrn + "contract").First();

                if (contract == null) return null;

                var hierarchyType = contract.Element(_nsUrn + "hierarchyType")?.Value;
                var fundingTypeCode = contract.Element(_nsUrn + "fundingType")?.Element(_nsUrn + "fundingTypeCode")?.Value;
                var status = contract.Element(_nsUrn + "contractStatus")?.Element(_nsUrn + "status")?.Value;
                var parentStatus = contract.Element(_nsUrn + "contractStatus")?.Element(_nsUrn + "parentStatus")?.Value;
                var updatedString = element.Element(_nsAtom + "updated")?.Value;
                var updated = DateTime.Parse(updatedString);
                var ukprn = contract.Element(_nsUrn + "contractor")?.Element(_nsUrn + "ukprn")?.Value;

                return new ContractFeedEvent
                {
                    Id = new Guid(id),
                    ProviderId = long.Parse(ukprn),
                    HierarchyType = hierarchyType,
                    FundingTypeCode = fundingTypeCode,
                    Status = status,
                    ParentStatus = parentStatus,
                    Updated = updated
                };
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Problem extracting contract feed event");
                return null;
            }
        }
    }
}
