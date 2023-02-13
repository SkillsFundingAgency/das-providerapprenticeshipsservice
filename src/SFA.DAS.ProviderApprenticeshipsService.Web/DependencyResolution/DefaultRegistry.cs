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
using FluentValidation;
using MediatR;
using SFA.DAS.Authorization.Context;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.Authorization.Handlers;
using SFA.DAS.Authorization.ProviderPermissions.Handlers;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.CookieService;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.Encoding;
using SFA.DAS.HashingService;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Data;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Caching;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Services;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Models;
using SFA.DAS.ProviderRelationships.Api.Client;
using SFA.DAS.ProviderRelationships.Api.Client.Configuration;
using StructureMap;
using SFA.DAS.ProviderApprenticeshipsService.Web;

namespace SFA.DAS.ProviderApprenticeshipsService.Web.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        private const string ServiceName = "SFA.DAS.ProviderApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry()
        {
            // the below 4 lines can all go with high confidence, azuretablestorage in startup replaces this all
            var environment = GetAndStoreEnvironment();
            var configurationRepository = GetConfigurationRepository();
            For<IConfigurationRepository>().Use(configurationRepository);
            var config = GetConfiguration(environment, configurationRepository);

            // this is basically the MAIN config,ned to ensure this has been done, it was surely done in the API
            For<IProviderAgreementStatusConfiguration>().Use(config);
            For<ProviderApprenticeshipsServiceConfiguration>().Use(config);

            // to be registered
            For<SFA.DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration>().Use(config.Features);
            For<IFeatureTogglesService<DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle>>().Use<FeatureTogglesService<DAS.Authorization.ProviderFeatures.Configuration.ProviderFeaturesConfiguration, DAS.Authorization.ProviderFeatures.Models.ProviderFeatureToggle>>();

            // only in PAS.API
            For<IAgreementStatusQueryRepository>().Use<ProviderAgreementStatusRepository>();

            // THIS IS USED IN eMPLOYERaCCOUNTService in Infrastructure so needs registered at Application or Infrastructure, aong with the config
            For<IAccountApiClient>().Use<AccountApiClient>();
            For<IAccountApiConfiguration>().Use<Domain.Configuration.AccountApiConfiguration>();

            //SFA.DAS.CookieService is incompatible...
            // it was used in the deleted CookieStorageService
            // SO THIS SEEMS LIKE A TASK TO REPLACE
            For(typeof(ICookieService<>)).Use(typeof(HttpCookieService<>));

            // when and why has CookieStorageService has been deleted???
            // it is used in various controllers in Web....
            For(typeof(ICookieStorageService<>)).Use(typeof(CookieStorageService<>));

            // registered but never used... need to check with Dan
            // the below has also been done in AuthorizationRegistry
            For<IAuthorizationContextProvider>().Use<AuthorizationContextProvider>();
            For<IAuthorizationHandler>().Use<AuthorizationHandler>();
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

        private static IConfigurationRepository GetConfigurationRepository()
        {
            return new AzureTableStorageConfigurationRepository(ConfigurationManager.AppSettings["ConfigurationStorageConnectionString"]);
        }

        private void PopulateSystemDetails(string envName)
        {
            SystemDetails.EnvironmentName = envName;
            SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }
}