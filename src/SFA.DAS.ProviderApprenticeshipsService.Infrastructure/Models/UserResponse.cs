using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models;

public class UserResponse
{
    [JsonRequired]
    public List<string> FamilyNames { get; set; }

    [JsonRequired]
    public List<string> GivenNames { get; set; }

    [JsonRequired]
    public List<string> Emails { get; set; }

    [JsonRequired]
    public List<string> Titles { get; set; }
}