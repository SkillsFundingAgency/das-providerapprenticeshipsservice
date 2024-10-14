using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Models.DfESignInUser;


public class DfeUser
{
    [JsonProperty("ukprn")]
    public string Ukprn { get; set; }

    [JsonProperty("users")]
    public List<User> Users { get; set; }
}

public class User
{
    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; }

    [JsonProperty("userStatus", NullValueHandling = NullValueHandling.Ignore)]
    public long? UserStatus { get; set; }

    [JsonProperty("roles")]
    public List<string> Roles { get; set; }
}