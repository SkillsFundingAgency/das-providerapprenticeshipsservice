﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Extensions.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.ContractFeed;

public sealed class ContractFeedProcessor : IContractDataProvider
{
    private readonly XNamespace _nsAtom = "http://www.w3.org/2005/Atom";
    private readonly XNamespace _nsUrn = "urn:sfa:schemas:contract";
    private readonly IContractFeedReader _reader;
    private readonly IContractFeedEventValidator _validator;
    private readonly ILogger<ContractFeedProcessor> _logger;

    public ContractFeedProcessor(
        IContractFeedReader reader, 
        IContractFeedEventValidator validator,
        ILogger<ContractFeedProcessor> logger)
    {
        _reader = reader;
        _validator = validator;
        _logger = logger;
    }

    public string FindPageWithBookmark(Guid? latestBookmark)
    {
        var latestBookmarkString = latestBookmark.HasValue ? latestBookmark.ToString() : "[not set]";

        _logger.LogInformation("Finding page for latest bookmark: {LatestBookmarkString}", latestBookmarkString);

        var currentPageUrl = _reader.LatestPageUrl;
        string startPageUrl = null;

        // Load page and check if contains bookmark
        _reader.Read(currentPageUrl, ReadDirection.Backward, (pageUrl, pageContent, pageNavigation) =>
        {
            // Is this the first page?
            if (pageNavigation.IsStartPage)
            {
                startPageUrl = pageUrl;
                _logger.LogInformation("Start page found at: {StartPageUrl}", startPageUrl);

                return false;
            }

            // Look at items on the page for a match of book mark
            var doc = XDocument.Parse(pageContent);

            if (PageContainsBookmark(latestBookmark, doc))
            {
                startPageUrl = pageUrl;
                _logger.LogInformation("Bookmark {LatestBookmarkString} found on page: {StartPageUrl}", latestBookmarkString, startPageUrl);

                return false;
            }

            _logger.LogInformation("Bookmark {LatestBookmarkString} not found on {PageUrl}", latestBookmarkString, pageUrl);

            return true;
        });

        return startPageUrl;
    }

    public int ReadEvents(string pageToReadUri, Guid? latestBookmark, Action<IList<ContractFeedEvent>, Guid?> action)
    {
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

            _logger.LogInformation($"Adding: {matchingContracts.Count} from page: {pageUri}");

            action(matchingContracts, newBookmark);

            contractCount += matchingContracts.Count;

            return true; // Continue reading more
        });

        return contractCount;
    }

    private bool PageContainsBookmark(Guid? latestBookmark, XDocument doc)
    {
        // process page in reverse order as items are provided in ascending datetime from fcs
        return doc.Descendants(_nsAtom + "entry")
            .Reverse()
            .Select(ExtractContractFeedEvent)
            .Any(x => x.Id == latestBookmark);
    }

    private ContractFeedEvent ExtractContractFeedEvent(XContainer element)
    {
        try
        {
            var id = element.Element(_nsAtom + "id")?.Value.Split(':').ElementAt(1);

            _logger.LogInformation($"Bookmark Id: {id}");

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
            _logger.LogWarning(ex, "Problem extracting contract feed event");
            return null;
        }
    }
}