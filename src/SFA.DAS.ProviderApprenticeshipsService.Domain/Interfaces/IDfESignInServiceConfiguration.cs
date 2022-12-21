using SFA.DAS.ProviderApprenticeshipsService.Domain.Models;

namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces
{
    public interface IDfESignInServiceConfiguration : IConfiguration
    {
        DfESignInConfig DfEOidcConfiguration { get; set; }
    }
}
