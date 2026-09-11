namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;

public interface IBaseConfiguration : IDatabaseConfiguration
{
    string ServiceBusConnectionString { get; set; }
    public string NServiceBusLicense { get; set; }
}