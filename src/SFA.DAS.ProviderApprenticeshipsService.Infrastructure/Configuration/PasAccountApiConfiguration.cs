using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;

namespace SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

public interface IPasAccountApiConfiguration : IBaseConfiguration
{
}

public class PasAccountApiConfiguration : IPasAccountApiConfiguration
{
    public string DatabaseConnectionString { get; set; }
    public string ServiceBusConnectionString { get; set; }
    public string NServiceBusLicense { get; set; }
}
