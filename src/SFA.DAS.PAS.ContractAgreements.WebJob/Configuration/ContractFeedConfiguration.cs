﻿using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.Configuration
{
    public class ContractFeedConfiguration : IConfiguration
    {
        public string AADInstance { get; set; }

        public string Tenant { get; set; }

        public string ClientId { get; set; }

        public string AppKey { get; set; }

        public string ResourceId { get; set; }

        public string BaseAddress { get; set; }

        public int ReadMaxPages { get; set; }

        // Needed?
        public string DatabaseConnectionString { get; set; }

        // Needed?
        public string ServiceBusConnectionString { get; set; }
    }
}