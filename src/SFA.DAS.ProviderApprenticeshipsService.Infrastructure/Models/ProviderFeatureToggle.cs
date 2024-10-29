using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

public class FeatureToggle
{
    public string Feature { get; set; }
    public bool IsEnabled { get; set; }
}

public class ProviderFeatureToggle : FeatureToggle
{
    public List<ProviderFeatureToggleWhitelistItem> Whitelist { get; set; }
    public bool IsWhitelistEnabled => Whitelist != null && Whitelist.Count > 0;

    public bool IsUserWhitelisted(long ukprn, string userEmail)
    {
        return Whitelist.Any(w => w.Ukprn == ukprn && (w.UserEmails == null || w.UserEmails.Count == 0 || w.UserEmails.Contains(userEmail, StringComparer.InvariantCultureIgnoreCase)));
    }
}