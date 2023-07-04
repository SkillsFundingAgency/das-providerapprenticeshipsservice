using System.IO;
using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.Helpers;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using IsDev = SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;


namespace SFA.DAS.ProviderApprenticeshipsService.Web;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;

        var config = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
        if (!configuration.IsDev())
        {
            config.AddJsonFile("appsettings.json", false)
                .AddJsonFile("appsettings.Development.json", true);
        }
#endif

        config.AddEnvironmentVariables();

        if (!configuration.IsDev())
        {
            config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                    options.ConfigurationKeysRawJsonResult = new[] { "SFA.DAS.Encoding" };
                }
            );
        }

        _configuration = config.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();

        services.AddOptions();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<DeleteRegisteredUserCommand>());

        services.AddLogging();
        services.AddApplicationServices(_configuration);
        services.AddOrchestrators();
        services.AddEncodingServices(_configuration);
        services.AddFeatureToggleService(_configuration);
        services.AddActionFilters();

        services.AddAndConfigureAuthentication(_configuration);
        services.AddAuthorizationServicePolicies();

        services.AddProviderUiServiceRegistration(_configuration);
        services.Configure<IISServerOptions>(options => { options.AutomaticAuthentication = false; });

        services.Configure<RouteOptions>(options =>
            {

            }).AddMvc(options =>
            {
                //options.AddAuthorization();
                options.Filters.Add<InvalidStateExceptionFilter>();
                options.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider());
                if (!_configuration.IsDev())
                {
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                }

            })
            .SetDfESignInConfiguration(_configuration.GetSection("UseDfESignIn").Get<bool>())
            .SetDefaultNavigationSection(NavigationSection.Home);

        services.AddApplicationInsightsTelemetry();

        services.AddDataProtection(_configuration);
        // Newtonsoft.Json is added for compatibility reasons
        // The recommended approach is to use System.Text.Json for serialization
        // Visit the following link for more guidance about moving away from Newtonsoft.Json to System.Text.Json
        // https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to
        // .AddNewtonsoftJson(options =>
        // {
        //     options.UseMemberCasing();
        // });
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
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}