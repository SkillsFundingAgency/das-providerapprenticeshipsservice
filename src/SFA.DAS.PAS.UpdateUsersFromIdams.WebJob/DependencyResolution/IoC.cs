using SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.DependencyResolution;
using StructureMap;

namespace SFA.DAS.PAS.UpdateUsersFromIdams.WebJob.DependencyResolution
{
    public class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
