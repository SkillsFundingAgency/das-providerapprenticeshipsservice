using Microsoft.Extensions.Configuration;
using SFA.DAS.DfESignIn.Auth.Helpers;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

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

        services.AddLogging();

        services.AddHttpContextAccessor();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<DeleteRegisteredUserCommand>());

        services.AddApplicationServices(_configuration);
        services.AddOrchestrators();
        services.AddEncodingServices(_configuration);
        services.AddFeatureToggleService(_configuration);
        services.AddActionFilters();

        services.AddAndConfigureAuthentication(_configuration);
        services.AddAuthorizationServicePolicies();

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
            .SetDfESignInConfiguration(_configuration.GetSection("UseDfESignIn").Get<bool>())
            .SetDefaultNavigationSection(NavigationSection.Home);

        services.AddApplicationInsightsTelemetry();

        services.AddDataProtection(_configuration);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseAuthentication();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });
    }
}