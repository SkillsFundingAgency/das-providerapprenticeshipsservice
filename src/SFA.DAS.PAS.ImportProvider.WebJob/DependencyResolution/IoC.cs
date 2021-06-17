using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using StructureMap;

namespace SFA.DAS.PAS.ImportProvider.WebJob.DependencyResolution
{
    public class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<ProviderApprenticeshipsServiceConfiguration>("SFA.DAS.ProviderApprenticeshipsService"));
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
