using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    /// <inheritdoc/>
    public class DfESignInServiceConfiguration : IDfESignInServiceConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public DfESignInConfig DfEOidcConfiguration { get; set; }

        [JsonProperty("DfEOidcConfiguration_ProviderRoATP")]
        public DfESignInClientConfig DfEOidcClientConfiguration { get; set; }
    }
}
