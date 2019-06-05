using SFA.DAS.AutoConfiguration;
using StructureMap;

namespace SFA.DAS.ProviderUrlHelper.StructureMap
{
    public class LinkGeneratorRegistry : Registry
    {
        /// <summary>
        /// Use this registry if you are using StructureMap for IoC.
        /// ProviderUrlConfiguration will be retrieved using AutoConfiguration.
        /// </summary>
        public LinkGeneratorRegistry()
        {
            For<ILinkGenerator>().Use<LinkGenerator>().Singleton();

            For<ProviderUrlConfiguration>().Use(ctx =>
                ctx.GetInstance<IAutoConfigurationService>().Get<ProviderUrlConfiguration>());
        }
    }
}
