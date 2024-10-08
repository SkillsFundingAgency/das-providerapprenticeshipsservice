﻿using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;

public interface IContractFeedConfiguration : IBaseConfiguration
{
    string AADInstance { get; set; }
    string Tenant { get; set; }
    string ClientId { get; set; }
    string ClientSecret { get; set; }
    string AppKey { get; set; }
    string ResourceId { get; set; }
    string BaseAddress { get; set; }
}

public class ContractFeedConfiguration : IContractFeedConfiguration
{
    public string AADInstance { get; set; }
    public string Tenant { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string AppKey { get; set; }
    public string ResourceId { get; set; }
    public string BaseAddress { get; set; }
    // Needed?
    public string DatabaseConnectionString { get; set; }
    // Needed?
    public string ServiceBusConnectionString { get; set; }
    public string NServiceBusLicense { get; set; }
}