using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Account.Api.ClientV2.Configuration;
using StructureMap;

namespace SFA.DAS.PAS.Account.Api.ClientV2.DependencyResolution
{
    public class PasAccountApiClientRegistry : Registry
    {
        public PasAccountApiClientRegistry()
        {
            For<IPasAccountApiClient>().Use(ctx => CreateClient(ctx)).Singleton();
        }

        private IPasAccountApiClient CreateClient(IContext ctx)
        {
            var config = GetConfig(ctx);
            var loggerFactory = ctx.GetInstance<ILoggerFactory>();

            var factory = new PasAccountApiClientFactory(config, loggerFactory);
            return factory.CreateClient();
        }

        private static PasAccountApiConfiguration GetConfig(IContext context)
        {
            var configuration = context.GetInstance<IConfiguration>();
            var configSection = configuration.GetSection(ConfigurationKeys.PasAccountApiClient);
            return configSection.Get<PasAccountApiConfiguration>();
        }
    }
}
