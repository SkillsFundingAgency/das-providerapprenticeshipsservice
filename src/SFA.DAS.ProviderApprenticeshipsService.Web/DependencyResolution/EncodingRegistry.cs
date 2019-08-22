using Microsoft.Azure;
using SFA.DAS.Configuration;
using SFA.DAS.Encoding;
using StructureMap;
using System;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class EncodingRegistry : Registry
    {
        public EncodingRegistry()
        {
            For<EncodingConfig>().Use<EncodingConfig>(ctx => GetConfig(ctx));
            For<IEncodingService>().Use<EncodingService>().Singleton();
        }

        public EncodingConfig GetConfig(IContext ctx)
        {
            var configRepo = ctx.GetInstance<IConfigurationRepository>();

            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationService = new ConfigurationService(configRepo,
                new ConfigurationOptions("SFA.DAS.Encoding", environment, "1.0"));

            return configurationService.Get<EncodingConfig>();
        }
    }
}