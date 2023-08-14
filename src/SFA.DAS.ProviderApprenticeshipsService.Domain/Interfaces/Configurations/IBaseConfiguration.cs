namespace SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

public interface IBaseConfiguration
{
    string DatabaseConnectionString { get; set; }
    string ServiceBusConnectionString { get; set; }
}