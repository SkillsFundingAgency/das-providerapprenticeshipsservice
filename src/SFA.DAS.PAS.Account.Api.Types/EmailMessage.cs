using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.PAS.Account.Api.Types
{
    //todo this needs to be referenced from employercommitments to replace the object of the same name there
    public class ProviderEmailRequest
    {
        public string TemplateId { get; set; }
        public Dictionary<string, string> Tokens { get; set; }
        public List<string> ExplicitEmailAddresses { get; set; }
    }
}
