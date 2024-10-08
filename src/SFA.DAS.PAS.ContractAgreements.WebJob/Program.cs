﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.ContractAgreements.WebJob.Extensions;

namespace SFA.DAS.PAS.ContractAgreements.WebJob;

public class Program
{
    public static async Task Main()
    {
        using var host = CreateHost();

        var logger = host.Services.GetService<ILogger<Program>>();

        logger.LogInformation("SFA.DAS.PAS.ContractAgreements.WebJob starting up ...");

        await host.RunAsync();
    }

    private static IHost CreateHost()
    {
        return new HostBuilder()
            .UseDasEnvironment()
            .AddConfiguration()
            .ConfigureDasLogging()
            .ConfigureDasWebJobs()
            .ConfigureServices()
            .Build();
    }
}