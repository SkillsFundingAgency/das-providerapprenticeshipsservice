using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration
{
    /// <summary>
    /// Model to read the DfESignIn configuration from Azure Table Storage.
    /// </summary>
    public class DfESignInServiceConfiguration : IDfESignInServiceConfiguration
    {
        public string DatabaseConnectionString { get; set; }
        public string ServiceBusConnectionString { get; set; }
        public DfESignInConfig DfEOidcConfiguration { get; set; }
    }
}
