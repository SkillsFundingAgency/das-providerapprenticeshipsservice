﻿using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Application.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces.Configurations;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

namespace SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;

public enum ServiceBusEndpointType
{
    Api,
    Web
}

[ExcludeFromCodeCoverage]
public static class NServiceBusServiceRegistrations
{
    public static void StartNServiceBus(this UpdateableServiceProvider services, bool isDevOrLocal, ServiceBusEndpointType endpointType)
    {
        var endPointName = $"SFA.DAS.ProviderApprenticeshipsService.{endpointType}";
        var pasConfiguration = services.GetService<IBaseConfiguration>();
       
        var databaseConnectionString = pasConfiguration.DatabaseConnectionString;

        if (string.IsNullOrEmpty(databaseConnectionString))
        {
            throw new InvalidConfigurationValueException("DatabaseConnectionString");
        }

        var allowOutboxCleanup = endpointType == ServiceBusEndpointType.Api;

        var endpointConfiguration = new EndpointConfiguration(endPointName)
            .UseErrorQueue($"{endPointName}-errors")
            .UseInstallers()
            .UseMessageConventions()
            .UseServicesBuilder(services)
            .UseNewtonsoftJsonSerializer()
            .UseOutbox(allowOutboxCleanup)
            .UseSqlServerPersistence(() => DatabaseExtensions.GetSqlConnection(databaseConnectionString))
            .ConfigureServiceBusTransport(() => pasConfiguration.ServiceBusConnectionString, isDevOrLocal)
            .UseUnitOfWork();

        if (!string.IsNullOrEmpty(pasConfiguration.NServiceBusLicense))
        {
            var decodedLicence = WebUtility.HtmlDecode(pasConfiguration.NServiceBusLicense);
            endpointConfiguration.License(decodedLicence);
        }

        var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();

        services.AddSingleton(p => endpoint)
            .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
            .AddHostedService<NServiceBusHostedService>();
    }  
}