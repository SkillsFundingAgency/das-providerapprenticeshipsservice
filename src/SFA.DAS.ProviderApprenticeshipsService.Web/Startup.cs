// This Startup file is based on ASP.NET Core new project templates and is included
// as a starting point for DI registration and HTTP request processing pipeline configuration.
// This file will need updated according to the specific scenario of the application being upgraded.
// For more information on ASP.NET Core startup files, see https://docs.microsoft.com/aspnet/core/fundamentals/startup

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SFA.DAS.Authorization.Mvc.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Models;
using SFA.DAS.Provider.Shared.UI.Startup;
using SFA.DAS.ProviderApprenticeshipsService.Application.Commands.DeleteRegisteredUser;
using SFA.DAS.ProviderApprenticeshipsService.Application.RegistrationExtensions;
using SFA.DAS.ProviderApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Configuration;
using SFA.DAS.ProviderApprenticeshipsService.Infrastructure.Logging;
using SFA.DAS.ProviderApprenticeshipsService.Web.Attributes;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authentication;
using SFA.DAS.ProviderApprenticeshipsService.Web.Authorization;
using SFA.DAS.ProviderApprenticeshipsService.Web.Exceptions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Extensions;
using SFA.DAS.ProviderApprenticeshipsService.Web.Middlewares;
using SFA.DAS.ProviderApprenticeshipsService.Web.ServiceRegistrations;

namespace SFA.DAS.ProviderApprenticeshipsService.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;

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
            
            services.AddMediatR(typeof(DeleteRegisteredUserCommand));
            
            services.AddLogging();
            services.AddApplicationServices(_configuration);
            services.AddOrchestrators();
            services.AddEncodingServices(_configuration);
            services.AddFeatureToggleService(_configuration);

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
                    options.Filters.Add<ProviderUkPrnCheckActionFilter>();
                    options.Filters.Add(new RoatpCourseManagementCheckActionFilter());
                    options.ModelBinderProviders.Insert(0, new TrimStringModelBinderProvider());
                    if (!_configuration.IsDev())
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    }

                })
                .SetDefaultNavigationSection(NavigationSection.YourCohorts);

            // Newtonsoft.Json is added for compatibility reasons
            // The recommended approach is to use System.Text.Json for serialization
            // Visit the following link for more guidance about moving away from Newtonsoft.Json to System.Text.Json
            // https://docs.microsoft.com/dotnet/standard/serialization/system-text-json-migrate-from-newtonsoft-how-to
            // .AddNewtonsoftJson(options =>
            // {
            //     options.UseMemberCasing();
            // });

            //services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
}
