using System.Collections.Generic;

namespace SFA.DAS.PAS.Account.Api.TypesV2
{
    public class ProviderEmailRequest
    {
        public string TemplateId { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public List<string> ExplicitEmailAddresses { get; set; }
    }
}
