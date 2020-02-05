using System.Collections.Generic;
using Newtonsoft.Json;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models
{
    public class UserResponse
    {
        [JsonProperty("ukprn")]
        public string Ukprn { get; set; }

        [JsonProperty("users")]
        public List<UkprnUser> Users { get; set; }
    }

    public class UkprnUser
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("roles")]
        public List<string> Roles { get; set; }
    }
}
