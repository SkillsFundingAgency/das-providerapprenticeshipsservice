using System;
using System.Configuration;
using System.Reflection;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Encoding;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class ProviderFeaturesRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.Roatp.CourseManagement.Web";
        private const string ServiceNamespace = "SFA.DAS";

        public ProviderFeaturesRegistry()
        {
            // Scan(
            //     scan => {
            //         scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
            //         // scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
            //         // scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
            //         // scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            //         // scan.RegisterConcreteTypesAgainstTheFirstInterface();
            //     });

            var environment = GetAndStoreEnvironment();
           var configurationRepository = GetConfigurationRepository();
            For<IConfigurationRepository>().Use(configurationRepository);

            var config = GetConfiguration(environment, configurationRepository);
            //var config = GetConfiguration(environment);
           // ConfigureHashingService(config);
            //For<IProviderFeaturesConfiguration>().Use(config);
            For<IProviderFeaturesConfiguration>().Use(config);

            //For<ICache>().Use<InMemoryCache>(); //RedisCache
            
            // For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            // For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            // For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));
            //
            // For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
            // For<IAuthorizationHandler>().Use<AuthorizationHandler>();

            //ConfigureFeatureToggle();

           // RegisterMediator();


            //For<EncodingConfig>().Use(x => GetEncodingConfig(environment, configurationRepository));
        }

        // private void ConfigureFeatureToggle()
        // {
        //     For<IBooleanToggleValueProvider>().Use<CloudConfigurationBooleanValueProvider>();
        //     For<IFeatureToggleService>().Use<FeatureToggleService>();
        // }



        // private void ConfigureHashingService(ProviderFeaturesConfiguration config)
        // {
        //     For<IHashingService>().Use(x => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
        //     For<IPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring));
        //     For<IAccountLegalEntityPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt));
        // }

        private IProviderCommitmentsLogger GetBaseLogger(IContext x)
        {
            var parentType = x.ParentType;
            return new ProviderCommitmentsLogger(new NLogLogger(parentType, x.GetInstance<ILoggingContext>()));
        }

        private string GetAndStoreEnvironment()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = ConfigurationManager.AppSettings["EnvironmentName"];
            }
            if (environment.Equals("LOCAL") || environment.Equals("AT") || environment.Equals("TEST"))
            {
                PopulateSystemDetails(environment);
            }

            return environment;
        }

        // private ProviderFeaturesConfiguration GetConfiguration(string environment)
        // {
        //     var configurationService = new ConfigurationService(null,
        //         new ConfigurationOptions(ServiceName, environment, "1.0"));
        //
        //     return configurationService.Get<ProviderFeaturesConfiguration>();
        // }

        private ProviderFeaturesConfiguration GetConfiguration(string environment, IConfigurationRepository configurationRepository)
        {
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));
        
            return configurationService.Get<ProviderFeaturesConfiguration>();
        }

        private EncodingConfig GetEncodingConfig(string environment, IConfigurationRepository configurationRepository)
        {
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions("SFA.DAS.Encoding", environment, "1.0"));

            return configurationService.Get<EncodingConfig>();
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
        }

        // private void RegisterMediator()
        // {
        //     For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
        //     For<IMediator>().Use<Mediator>();
        // }

        private void PopulateSystemDetails(string envName)
        {
            SystemDetails.EnvironmentName = envName;
            SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}
