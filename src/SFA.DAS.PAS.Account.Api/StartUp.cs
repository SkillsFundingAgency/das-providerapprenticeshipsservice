using Microsoft.Extensions.Configuration;
using SFA.DAS.Api.Common.Infrastructure;
using SFA.DAS.PAS.Account.Api.Authentication;
using SFA.DAS.PAS.Account.Api.Authorization;
using SFA.DAS.PAS.Account.Api.ServiceRegistrations;
using SFA.DAS.PAS.Account.Application.Queries.GetAccountUsers;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;

namespace SFA.DAS.PAS.Account.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration.BuildDasConfiguration();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var isDevOrLocal = _configuration.IsDevOrLocal();
        
        services.AddLogging();
        services.AddApiAuthentication(_configuration);
        services.AddApiAuthorization(isDevOrLocal);
        services.AddOptions();
        services.AddConfigurationOptions(_configuration);
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAccountUsersHandler>());
        services.AddOrchestrators();
        services.AddDataRepositories();
        services.AddApiValidators();
        services.AddApplicationServices();
        services.AddNotifications(_configuration);

        if (!isDevOrLocal)
        {
            services.AddHealthChecks();
        }

        services.AddMvc(o =>
            {
                if (!isDevOrLocal)
                {
                    o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                }
                o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        services.AddDasSwagger();
        services.AddApiVersioning(opt => opt.ApiVersionReader = new HeaderApiVersionReader("X-Version"));
        services.AddApplicationInsightsTelemetry();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        if (!env.IsDevelopment())
        {
            app.UseHealthChecks();
        }

        app.UseHttpsRedirection()
            .UseAuthentication()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "api/{controller=Users}/{action=Index}/{id?}");
            })
            .UseSwagger()
            .UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "PAS Account API v1");
                opt.RoutePrefix = string.Empty;
            });
    }
}