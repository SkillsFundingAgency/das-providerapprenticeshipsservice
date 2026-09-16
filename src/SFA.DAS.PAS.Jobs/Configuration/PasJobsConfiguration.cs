using System.Diagnostics.CodeAnalysis;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;

namespace SFA.DAS.PAS.Jobs.Configuration;

[ExcludeFromCodeCoverage]
public class PasJobsConfiguration : IDatabaseConfiguration
{
    public string DatabaseConnectionString { get; set; }
    public RoatpApiConfiguration RoatpApiClient { get; set; }
}
