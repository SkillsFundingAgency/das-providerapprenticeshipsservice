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
using System.Net.Http;
using System.Reflection;
using System.Web;
using FluentValidation;
using MediatR;
using FeatureToggle;
using Microsoft.Azure;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Client.Configuration;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.CookieService;
using SFA.DAS.Http;
using SFA.DAS.Http.TokenGenerators;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Notifications.Api.Client;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.BulkUploadApprenticeships;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderApprenticeshipsService.Web.Orchestrators.BulkUpload;
using StructureMap;
using StructureMap.Graph;
using SFA.DAS.Learners.Validators;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation;
using SFA.DAS.ProviderApprenticeshipsService.Web.Validation.Text;

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
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                    scan.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scan.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            var config = GetConfiguration();

            ConfigureHashingService(config);
            ConfigureCommitmentsApi(config);
            ConfigureNotificationsApi(config);

            For<IApprenticeshipInfoServiceConfiguration>().Use(config.ApprenticeshipInfoService);
            For<IConfiguration>().Use(config);
            For<ICache>().Use<InMemoryCache>(); //RedisCache
            For<IAgreementStatusQueryRepository>().Use<ProviderAgreementStatusRepository>();
            For<IApprenticeshipValidationErrorText>().Use<WebApprenticeshipValidationText>();
            For<IApprenticeshipCoreValidator>().Use<ApprenticeshipCoreValidator>().Singleton();
            For<IApprovedApprenticeshipValidator>().Use<ApprovedApprenticeshipValidator>().Singleton();

            For<HttpContextBase>().Use(() => new HttpContextWrapper(HttpContext.Current));
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));

            ConfigureFeatureToggle();

            RegisterMediator();
            ConfigureLogging();

            ConfigureInstrumentedTypes();
        }

        private void ConfigureFeatureToggle()
        {
            For<IBooleanToggleValueProvider>().Use<CloudConfigurationBooleanValueProvider>();
            For<IFeatureToggleService>().Use<FeatureToggleService>();
        }

        private void ConfigureCommitmentsApi(ProviderApprenticeshipsServiceConfiguration config)
        {
            var bearerToken = (IGenerateBearerToken)new JwtBearerTokenGenerator(config.CommitmentsApi);

            var httpClient = new HttpClientBuilder()
                .WithBearerAuthorisationHeader(bearerToken)
                .WithHandler(new NLog.Logger.Web.MessageHandlers.RequestIdMessageRequestHandler())
                .WithHandler(new NLog.Logger.Web.MessageHandlers.SessionIdMessageRequestHandler())
                .WithDefaultHeaders()
                .Build();

            For<IProviderCommitmentsApi>().Use<ProviderCommitmentsApi>()
                .Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi)
                .Ctor<HttpClient>().Is(httpClient);

            For<IValidationApi>().Use<ValidationApi>()
                .Ctor<ICommitmentsApiClientConfiguration>().Is(config.CommitmentsApi)
                .Ctor<HttpClient>().Is(httpClient);
        }

        private void ConfigureNotificationsApi(ProviderApprenticeshipsServiceConfiguration config)
        {
            var bearerToken = string.IsNullOrWhiteSpace(config.NotificationApi.ClientId)
                    ? (IGenerateBearerToken)new JwtBearerTokenGenerator(config.NotificationApi)
                    : new AzureADBearerTokenGenerator(config.NotificationApi);

            var httpClient = new HttpClientBuilder()
                .WithBearerAuthorisationHeader(bearerToken)
                .WithHandler(new NLog.Logger.Web.MessageHandlers.RequestIdMessageRequestHandler())
                .WithHandler(new NLog.Logger.Web.MessageHandlers.SessionIdMessageRequestHandler())
                .WithDefaultHeaders()
                .Build();

            For<INotificationsApi>().Use<NotificationsApi>().Ctor<HttpClient>().Is(httpClient);

            For<INotificationsApiClientConfiguration>().Use(config.NotificationApi);
        }

        private void ConfigureInstrumentedTypes()
        {
            For<IBulkUploadValidator>().Use(x => new InstrumentedBulkUploadValidator(x.GetInstance<ILog>(), x.GetInstance<BulkUploadValidator>(), x.GetInstance<IUlnValidator>(), x.GetInstance<IAcademicYearDateProvider>()));
            For<IBulkUploadFileParser>().Use(x => new InstrumentedBulkUploadFileParser(x.GetInstance<ILog>(), x.GetInstance<BulkUploadFileParser>()));

            For<IAsyncRequestHandler<BulkUploadApprenticeshipsCommand, Unit>>()
                .Use(x => new InstrumentedBulkUploadApprenticeshipsCommandHandler(x.GetInstance<IProviderCommitmentsLogger>(), x.GetInstance<BulkUploadApprenticeshipsCommandHandler>()));
        }

        private void ConfigureLogging()
        {
            For<IRequestContext>().Use(x => new RequestContext(new HttpContextWrapper(HttpContext.Current)));
            For<IProviderCommitmentsLogger>().Use(x => GetBaseLogger(x)).AlwaysUnique();
            For<ILog>().Use(x => new NLogLogger(
                x.ParentType,
                x.GetInstance<IRequestContext>(),
                null)).AlwaysUnique();
        }

        private void ConfigureHashingService(ProviderApprenticeshipsServiceConfiguration config)
        {
            For<IHashingService>().Use(x => new HashingService.HashingService(config.AllowedHashstringCharacters, config.Hashstring));
        }

        private IProviderCommitmentsLogger GetBaseLogger(IContext x)
        {
            var parentType = x.ParentType;
            return new ProviderCommitmentsLogger(new NLogLogger(parentType, x.GetInstance<IRequestContext>()));
        }

        private ProviderApprenticeshipsServiceConfiguration GetConfiguration()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }
            if (environment.Equals("LOCAL") || environment.Equals("AT") || environment.Equals("TEST"))
            {
                PopulateSystemDetails(environment);
            }

            var configurationRepository = GetConfigurationRepository();
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            var result = configurationService.Get<ProviderApprenticeshipsServiceConfiguration>();

            return result;
        }

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
        }

        private void RegisterMediator()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
        }

        private void PopulateSystemDetails(string envName)
        {
            SystemDetails.EnvironmentName = envName;
            SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}