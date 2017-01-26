using SFA.DAS.PAS.ContractAgreements.WebJob.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;

using StructureMap;

namespace SFA.DAS.PAS.ContractAgreements.WebJob.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<ContractFeedConfiguration>("SFA.DAS.ContractAgreements"));
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
