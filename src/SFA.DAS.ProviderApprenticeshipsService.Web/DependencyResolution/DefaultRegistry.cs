// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultRegistry.cs" company="Web Advanced">
// Copyright 2012 Web Advanced (www.webadvanced.com)
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FeatureToggle;
using FluentValidation;
using MediatR;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.ProviderPermissions.Handlers;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.HashingService;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.Learners.Validators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NLog.Logger.Web.MessageHandlers;
using SFA.DAS.ProviderApprenticeshipsService.Application.Services;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.ProviderRelationships.Api.Client;
using StructureMap;
using System;
using System.Configuration;
using System.Net.Http;
using System.Reflection;
using System.Web;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.Caches;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class DefaultRegistry : Registry {
        private const string ServiceName = "SFA.DAS.ProviderApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                    scan.ConnectImplementationsToTypesClosing(typeof(IValidator<>)).OnAddedPluginTypes(t => t.Singleton());
                    scan.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            var environment = GetAndStoreEnvironment();
            var configurationRepository = GetConfigurationRepository();

            For<IConfigurationRepository>().Use(configurationRepository);

            var config = GetConfiguration(environment, configurationRepository);

            #region uncomment to override the auto config of the ProviderRelationships Api Client

            //var providerPermissionsReadStoreConfig = GetProviderPermissionsReadStoreConfiguration(environment, configurationRepository);
            //For<ProviderRelationshipsReadStoreConfiguration>().ClearAll();
            //For<ProviderRelationshipsReadStoreConfiguration>().Use(providerPermissionsReadStoreConfig);

            //var providerRelationshipsApiClientConfiguration =
            //    GetProviderRelationshipsApiClientConfiguration(environment, configurationRepository);
            //For<ProviderRelationshipsApiClientConfiguration>().ClearAll();
            //For<ProviderRelationshipsApiClientConfiguration>().Use(providerRelationshipsApiClientConfiguration);

            #endregion

            ConfigureHashingService(config);
            For<IProviderAgreementStatusConfiguration>().Use(config);
            For<ProviderApprenticeshipsServiceConfiguration>().Use(config);

            For<IContentClientApiConfiguration>().Use(config.ContentApi);
            For<IClientContentApiClient>().Use<ClientContentApiClient>().Ctor<HttpClient>().Is(c => CreateClient(c, config));

            For<IAgreementStatusQueryRepository>().Use<ProviderAgreementStatusRepository>();
            For<IApprenticeshipValidationErrorText>().Use<WebApprenticeshipValidationText>();
            For<IApprenticeshipCoreValidator>().Use<ApprenticeshipCoreValidator>().Singleton();
            For<IApprovedApprenticeshipValidator>().Use<ApprovedApprenticeshipValidator>().Singleton();
            For<IAccountApiClient>().Use<AccountApiClient>();
            For<IAccountApiConfiguration>().Use<Domain.Configuration.AccountApiConfiguration>();

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));


            For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
            For<IAuthorizationHandler>().Use<AuthorizationHandler>();

            
            For<IDistributedCache>().Use(c => c.GetInstance<IEnvironmentService>().IsCurrent(DasEnv.LOCAL)
                ? new LocalDevCache() as IDistributedCache
                : new RedisCache(ConfigurationManager.AppSettings["RedisConnection"].ToString()) as IDistributedCache).Singleton();
            
            ConfigureFeatureToggle();

            RegisterMediator();

            ConfigureProviderRelationshipsApiClient();

            ConfigureLogging();

            ConfigureInstrumentedTypes();

            For<EncodingConfig>().Use(x => GetEncodingConfig(environment, configurationRepository));
        }

        private void ConfigureFeatureToggle()
        {
            For<IBooleanToggleValueProvider>().Use<CloudConfigurationBooleanValueProvider>();
            For<IFeatureToggleService>().Use<FeatureToggleService>();
        }

        private void ConfigureInstrumentedTypes()
        {
            For<IBulkUploadValidator>().Use(x => new InstrumentedBulkUploadValidator(x.GetInstance<ILog>(), x.GetInstance<BulkUploadValidator>(), x.GetInstance<IUlnValidator>(), x.GetInstance<IAcademicYearDateProvider>()));
            For<IBulkUploadFileParser>().Use(x => new InstrumentedBulkUploadFileParser(x.GetInstance<ILog>(), x.GetInstance<BulkUploadFileParser>()));
        }

        private void ConfigureLogging()
        {
            For<ILoggingContext>().Use(x => new RequestContext(new HttpContextWrapper(HttpContext.Current)));
            For<IProviderCommitmentsLogger>().Use(x => GetBaseLogger(x)).AlwaysUnique();
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                x.GetInstance<ILoggingContext>(),
                null)).AlwaysUnique();
        }

        private void ConfigureHashingService(ProviderApprenticeshipsServiceConfiguration config)
        {
            For<IHashingService>().Use(x => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
            For<IPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedHashstringCharacters, config.PublicHashstring));
            For<IAccountLegalEntityPublicHashingService>().Use(x => new PublicHashingService(config.PublicAllowedAccountLegalEntityHashstringCharacters, config.PublicAllowedAccountLegalEntityHashstringSalt));
        }

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

        private ProviderApprenticeshipsServiceConfiguration GetConfiguration(string environment, IConfigurationRepository configurationRepository)
        {
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            return configurationService.Get<ProviderApprenticeshipsServiceConfiguration>();
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

        private void RegisterMediator()
        {
            For<ServiceFactory>().Use<ServiceFactory>(ctx => ctx.GetInstance);
            For<IMediator>().Use<Mediator>();
        }

        private void PopulateSystemDetails(string envName)
        {
            SystemDetails.EnvironmentName = envName;
            SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private void ConfigureProviderRelationshipsApiClient()
        {
            var useStub = GetUseStubProviderRelationshipsSetting();

            if (useStub)
            {
                For<IProviderRelationshipsApiClient>().Use<StubProviderRelationshipsApiClient>();
            }
        }

        private bool GetUseStubProviderRelationshipsSetting()
        {
            var value = ConfigurationManager.AppSettings["UseStubProviderRelationships"];

            if (value == null)
            {
                return false;
            }

            if (!bool.TryParse(value, out var result))
            {
                return false;
            }

            return result;
        }


        private HttpClient CreateClient(IContext context, ProviderApprenticeshipsServiceConfiguration providerApprenticeshipsServiceConfiguration)
        {
            var config = providerApprenticeshipsServiceConfiguration.ContentApi;

            HttpClient httpClient = new HttpClientBuilder()
                    .WithBearerAuthorisationHeader(new AzureActiveDirectoryBearerTokenGenerator(config))
                    .WithHandler(new RequestIdMessageRequestHandler())
                    .WithHandler(new SessionIdMessageRequestHandler())
                    .WithDefaultHeaders()
                    .Build();


            return httpClient;
        }
    }
}
