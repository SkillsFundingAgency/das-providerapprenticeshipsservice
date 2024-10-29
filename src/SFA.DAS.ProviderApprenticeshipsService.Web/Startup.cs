using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.ApplicationInsights;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Features.ClientOutbox.Data;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderApprenticeshipsService.Application;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;
using SFA.DAS.UnitOfWork.DependencyResolution.Microsoft;
using SFA.DAS.UnitOfWork.Mvc.Extensions;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.Microsoft;

namespace SFA.DAS.ProviderApprenticeshipsService.Web;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(_configuration);
        services.AddOptions();
        services.AddConfigurationOptions(_configuration);
        services.AddLogging(builder =>
        {
            builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
            builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
        });

        services.AddHttpContextAccessor();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<DeleteRegisteredUserCommand>());

        services.AddApplicationServices(_configuration);
        services.AddOrchestrators();
        services.AddEncodingServices(_configuration);
        services.AddFeatureToggleService();
        services.AddActionFilters();

        services.AddAndConfigureAuthentication(_configuration);
        services.AddAuthorizationServicePolicies();

        services
            .AddUnitOfWork()
            .AddNServiceBusClientUnitOfWork();

        services.AddProviderUiServiceRegistration(_configuration);
        services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

        services.Configure<RouteOptions>(_ => { }).AddMvc(options =>
            {
                options.Filters.Add<InvalidStateExceptionFilter>();
                options.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider());

                if (!_configuration["EnvironmentName"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                }
            })
            .SetDfESignInConfiguration(true)
            .EnableCookieBanner()
            .SetDefaultNavigationSection(NavigationSection.Home);

        services.AddApplicationInsightsTelemetry();

        services.AddDataProtection(_configuration);
    }
    
    public void ConfigureContainer(UpdateableServiceProvider serviceProvider)
    {
        serviceProvider.StartNServiceBus(_configuration.IsDevOrLocal(), ServiceBusEndpointType.Web);

        // Replacing ClientOutboxPersisterV2 with a local version to fix unit of work issue due to propogating Task up the chain rather than awaiting on DB Command.
        // not clear why this fixes the issue. Attempted to make the change in SFA.DAS.Nservicebus.SqlServer however it conflicts when upgraded with SFA.DAS.UnitOfWork.Nservicebus
        // which would require upgrading to NET6 to resolve.
        var serviceDescriptor = serviceProvider.FirstOrDefault(serv => serv.ServiceType == typeof(IClientOutboxStorageV2));
        serviceProvider.Remove(serviceDescriptor);
        serviceProvider.AddScoped<IClientOutboxStorageV2, ClientOutboxPersisterV2>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthentication();
        app.UseUnitOfWork();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}