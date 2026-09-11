using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SFA.DAS.PAS.Jobs.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Extensions;

namespace SFA.DAS.PAS.Jobs;

[ExcludeFromCodeCoverage]
public static class Program
{
    public static async Task Main(string[] args)
    {
        FunctionsApplicationBuilder builder = FunctionsApplication.CreateBuilder(args);

        builder.AddConfiguration();

        builder.ConfigureFunctionsWebApplication();

        builder.Services
            .AddApplicationInsightsTelemetryWorkerService()
            .ConfigureFunctionsApplicationInsights()
            .AddTelemetryRegistration((IConfigurationRoot)builder.Configuration)
            .AddApplicationRegistrations(builder.Configuration);

        builder.Services.Configure<LoggerFilterOptions>(options =>
        {
            var defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");

            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });

        var app = builder.Build();

        await app.RunAsync();
    }
}
