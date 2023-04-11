using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SFA.DAS.Api.Common.Infrastructure;
using System.Text.Json.Serialization;
using SFA.DAS.PAS.Account.Api.ServiceRegistrations;
using SFA.DAS.PAS.Account.Api.Authentication;
using SFA.DAS.PAS.Account.Api.Authorization;
using System;
using System.Collections.Generic;

namespace SFA.DAS.PAS.Account.Api
{
    public class StartUp
    {
        public IConfiguration _configuration { get; }
        private readonly IHostEnvironment _environment;

        public StartUp(IConfiguration configuration, IHostEnvironment environment)
        {
            _environment = environment;
            _configuration = configuration.BuildDasConfiguration();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var isDevOrLocal = _configuration.IsDevOrLocal();

            services
                .AddApiAuthentication(_configuration)
                .AddApiAuthorization(isDevOrLocal);

            services.AddOptions();
            services.AddConfigurationOptions(_configuration);
            services.AddMediatRHandlers();
            services.AddOrchestrators();
            services.AddDataRepositories();
            services.AddFluentValidation();
            services.AddApplicationServices();
            services.AddNotifications(_configuration);

            if (_configuration["EnvironmentName"] != "DEV")
            {
                services.AddHealthChecks();
            }

           services
                .AddMvc(o =>
                {
                    if (!(_configuration["EnvironmentName"]!.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase) ||
                          _configuration["EnvironmentName"]!.Equals("DEV", StringComparison.CurrentCultureIgnoreCase)))
                    {
                        o.Conventions.Add(new AuthorizeControllerModelConvention(new List<string>()));
                    }
                    o.Conventions.Add(new ApiExplorerGroupPerVersionConvention());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddApplicationInsightsTelemetry();

            services.AddSwaggerGen(c =>
            {
                c.OperationFilter<SwaggerVersionHeaderFilter>();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PasAccountApi", Version = "v1" });
            });

            services.AddApiVersioning(opt => {
                opt.ApiVersionReader = new HeaderApiVersionReader("X-Version");
            });

            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }   

            if (!env.IsDevelopment())
            {
                app.UseHealthChecks();
            };

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
}
