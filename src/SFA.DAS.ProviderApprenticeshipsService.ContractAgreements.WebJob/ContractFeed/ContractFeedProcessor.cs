using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedProcessor : IContractDataProvider
    {
        private readonly XNamespace _nsAtom = "http://www.w3.org/2005/Atom";
        private readonly XNamespace _nsUrn = "urn:sfa:schemas:contract";

        private readonly ContractFeedReader _reader;

        private readonly IContractFeedEventValidator _validator;

        private readonly ILog _logger;

        public ContractFeedProcessor(
            ContractFeedReader reader, 
            IContractFeedEventValidator validator, 
            ILog logger)
        {
            _reader = reader;
            _validator = validator;
            _logger = logger;
        }

        public void ReadEvents(int lastPageNumber, Guid lastBookmarkedItemId, Action<int, IEnumerable<ContractFeedEvent>> saveRecordsAction)
        {
            _reader.Read(lastPageNumber, (pageNumber, pageContent) =>
            {
                var doc = XDocument.Parse(pageContent);

                // process page in reverse order as items are provided in ascending datetime from fcs
                var entries = doc.Descendants(_nsAtom + "entry").Reverse().ToList();

                var newContractDataPage = entries
                    .Select(ExtractContractFeedEvent)
                    .Where(contractFeedEvent => contractFeedEvent != null)
                    .TakeWhile(contractFeedEvent => lastBookmarkedItemId == Guid.Empty || contractFeedEvent.Id != lastBookmarkedItemId)
                    .Where(_validator.Validate) // ToDo: Enable when testing
                    .ToList();

                _logger.Info($"Adding: {newContractDataPage.Count} from page: {pageNumber}");
                saveRecordsAction(pageNumber, newContractDataPage);
            });
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
