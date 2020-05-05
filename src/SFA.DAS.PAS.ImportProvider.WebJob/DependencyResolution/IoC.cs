using StructureMap;

namespace SFA.DAS.PAS.ImportProvider.WebJob.DependencyResolution
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
