using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.ContractFeed;

public class ContractFeedEvent
{
    public Guid Id { get; set; }
    public long ProviderId { get; set; }
    public string HierarchyType { get; set; }
    public string FundingTypeCode { get; set; }
    public string Status { get; set; }
    public string ParentStatus { get; set; }
    public DateTime Updated { get; set; }

    public int PageNumber { get; set; }
}