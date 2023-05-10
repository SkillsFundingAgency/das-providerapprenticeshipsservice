using Newtonsoft.Json;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    /// <summary>
    /// Contract to read the DfESignIn configuration from Azure Table Storage.
    /// </summary>
    public interface IDfESignInServiceConfiguration : IConfiguration
    {
        DfESignInConfig DfEOidcConfiguration { get; set; }

        [JsonProperty("DfEOidcConfiguration_ProviderRoATP")]
        DfESignInClientConfig DfEOidcClientConfiguration { get; set;}
    }
}
