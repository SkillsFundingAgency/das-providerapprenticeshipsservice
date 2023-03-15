using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Models
{
    public class UserResponse
    {
        [JsonRequiredAttribute]
        public List<string> FamilyNames { get; set; }

        [JsonRequiredAttribute]
        public List<string> GivenNames { get; set; }

        [JsonRequiredAttribute]
        public List<string> Emails { get; set; }

        [JsonRequiredAttribute]
        public List<string> Titles { get; set; }
    }
}
