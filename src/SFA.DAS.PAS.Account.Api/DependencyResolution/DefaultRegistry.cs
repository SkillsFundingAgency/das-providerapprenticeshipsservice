﻿// --------------------------------------------------------------------------------------------------------------------
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
using System.Linq;
using System.Reflection;
using System.Web;

using AutoMapper;
using MediatR;

using Microsoft.Azure;

using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.NLog.Logger;
using SFA.DAS.ProviderApprenticeshipsService.Domain;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;

using StructureMap;
using WebGrease.Css.Extensions;
using IConfiguration = SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.IConfiguration;

namespace SFA.DAS.PAS.Account.Api.DependencyResolution {
    using StructureMap.Configuration.DSL;
    using StructureMap.Graph;


    public class DefaultRegistry : Registry {
        private const string ServiceName = "SFA.DAS.ProviderApprenticeshipsService";
        private const string ServiceNamespace = "SFA.DAS";

        public DefaultRegistry() {
            Scan(
                scan => {
                    scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith(ServiceNamespace));
                    scan.RegisterConcreteTypesAgainstTheFirstInterface();
                });

            var config = GetConfiguration();

            For<IConfiguration>().Use(config);

            RegisterMediator();
            ConfigureLogging();
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
                // Not sure how/when/why this is used?
                //SystemDetails.EnvironmentName = envName;
                //SystemDetails.VersionNumber = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            var configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            var configurationService = new ConfigurationService(configurationRepository,
                new ConfigurationOptions(ServiceName, environment, "1.0"));

            var result = configurationService.Get<ProviderApprenticeshipsServiceConfiguration>();

            return result;
        }

        private void RegisterMediator()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
            For<IMediator>().Use<Mediator>();
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

        private IProviderCommitmentsLogger GetBaseLogger(IContext x)
        {
            var parentType = x.ParentType;
            return new ProviderCommitmentsLogger(new NLogLogger(parentType, x.GetInstance<IRequestContext>()));
        }
    }
}