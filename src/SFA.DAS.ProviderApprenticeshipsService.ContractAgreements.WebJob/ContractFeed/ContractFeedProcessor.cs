using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using FluentValidation;

using SFA.DAS.ProviderApprenticeshipsService.Domain;

namespace SFA.DAS.ProviderApprenticeshipsService.ContractAgreements.WebJob.ContractFeed
{
    public class ContractFeedProcessor : IContractDataProvider
    {
        private readonly XNamespace _nsAtom = "http://www.w3.org/2005/Atom";
        private readonly XNamespace _nsUrn = "urn:sfa:schemas:contract";

        private readonly ContractFeedReader _reader;

        private readonly AbstractValidator<ContractFeedEvent> _validator;

        public ContractFeedProcessor(ContractFeedReader reader, AbstractValidator<ContractFeedEvent> validator)
        {
            _reader = reader;
            _validator = validator;
        }

        public void ReadEvents(Guid lastBookmarkedItemId, Action<int, IEnumerable<ContractFeedEvent>> pageHandler)
        {
            _reader.Read((pageNumber, pageContent) =>
            {
                var doc = XDocument.Parse(pageContent);

                // process page in reverse order as items are provided in ascending datetime from fcs
                var entries = doc.Descendants(_nsAtom + "entry").Reverse().ToList();

                var foundLastBookmarkedItem = false;

                var newContractDataPage = new List<ContractFeedEvent>();

                foreach (var entry in entries)
                {
                    var contractFeedEvent = ExtractContractFeedEvent(entry);

                    if (contractFeedEvent == null) continue;

                    if (lastBookmarkedItemId != Guid.Empty && contractFeedEvent.Id == lastBookmarkedItemId)
                    {
                        foundLastBookmarkedItem = true;
                        break; // already processed this item so ignore it and the remainder of entries in this page (as they will be older)
                    }

                    if(_validator.Validate(contractFeedEvent).IsValid)
                    {
                        newContractDataPage.Add(contractFeedEvent);
                    }
                }

                pageHandler(pageNumber, newContractDataPage);

                return !foundLastBookmarkedItem; // return true means more pages should be processed
            });
        }

        private ContractFeedEvent ExtractContractFeedEvent(XContainer element)
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
    }

    public interface IContractDataProvider
    {
        void ReadEvents(Guid lastBookmarkedItemId, Action<int, IEnumerable<ContractFeedEvent>> pageHandler);
    }
}
