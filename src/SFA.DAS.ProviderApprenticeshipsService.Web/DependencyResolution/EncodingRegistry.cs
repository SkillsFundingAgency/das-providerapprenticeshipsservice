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
            For<EncodingConfig>().Use(ctx => GetConfig(ctx)).Singleton();
            For<IEncodingService>().Use<EncodingService>().Singleton();
        }

        public EncodingConfig GetConfig(IContext ctx)
        {
            var configRepo = ctx.GetInstance<IConfigurationRepository>();

            var environment = GetEnvironment();

            var configurationService = new ConfigurationService(configRepo,
                new ConfigurationOptions("SFA.DAS.Encoding", environment, "1.0"));

            return configurationService.Get<EncodingConfig>();
        }

        private static string GetEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            return environment;
        }
    }
}