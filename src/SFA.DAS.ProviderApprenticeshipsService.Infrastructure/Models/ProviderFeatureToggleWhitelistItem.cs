using System.Collections.Generic;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

public class ProviderFeatureToggleWhitelistItem
{
    public long Ukprn { get; set; }
    public List<string> UserEmails { get; set; }
}