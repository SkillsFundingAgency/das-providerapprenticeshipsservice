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

using System;
using System.Configuration;
using System.Reflection;
using System.Web;
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
using SFA.DAS.Learners.Validators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;
using StructureMap;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            Scan(
                scan =>
                {
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
            ConfigureHashingService(config);
            For<IProviderAgreementStatusConfiguration>().Use(config);
            For<ProviderApprenticeshipsServiceConfiguration>().Use(config);

            For<ICache>().Use<InMemoryCache>(); //RedisCache
            For<IAgreementStatusQueryRepository>().Use<ProviderAgreementStatusRepository>();
            For<IApprenticeshipValidationErrorText>().Use<WebApprenticeshipValidationText>();
            For<IAccountApiClient>().Use<AccountApiClient>();
            For<IAccountApiConfiguration>().Use<Domain.Configuration.AccountApiConfiguration>();

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));

            For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
            For<IAuthorizationHandler>().Use<AuthorizationHandler>();

            ConfigureFeatureToggle();

            RegisterMediator();

            ConfigureProviderRelationshipsApiClient();

            ConfigureLogging();

            For<EncodingConfig>().Use(x => GetEncodingConfig(environment, configurationRepository));
        }

        private void ConfigureFeatureToggle()
        {
            For<IBooleanToggleValueProvider>().Use<CloudConfigurationBooleanValueProvider>();
            For<IFeatureToggleService>().Use<FeatureToggleService>();
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
            For<ProviderRelationshipsApiConfiguration>().Use(c => c.GetInstance<ProviderApprenticeshipsServiceConfiguration>().ProviderRelationshipsApi);

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
    }
}