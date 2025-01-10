namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;

public interface IBaseConfiguration
{
    string DatabaseConnectionString { get; set; }
    string ServiceBusConnectionString { get; set; }
    public string NServiceBusLicense { get; set; }
}