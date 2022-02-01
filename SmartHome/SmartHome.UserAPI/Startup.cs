using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using SmartHome.Stardog.Interfaces;
using SmartHome.Stardog.Services;
using SmartHome.API.Swagger;
using System.Linq;

namespace SmartHome.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.ConfigureLogging(Configuration);
            services.AddSingleton(log => Log.Logger);
            services.AddSingleton(data => ConfigurationObjectBuilder.BuildStardogData(Configuration));
            services.AddScoped<IUserService,UserService>();
            services.AddScoped<IGroupsService, GroupService>();
            services.AddScoped<ThingsService>();
            services.AddAuthentication(AzureADDefaults.BearerAuthenticationScheme)
                .AddAzureADBearer(options => Configuration.Bind("AzureAd", options));
            services.AddControllers();
            services.AddSwaggerGen(x =>
            {
                x.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                x.SwaggerDoc("v1", new OpenApiInfo() { Title = "User API", Description = "An API used to retrieve and alter user-related data.", Version = "v1" });
            });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            var swaggerOptions = new SwaggerOptions();
            Configuration.GetSection(nameof(swaggerOptions)).Bind(swaggerOptions);
            app.UseSwagger(option => option.RouteTemplate = swaggerOptions.JsonRoute);
            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint(swaggerOptions.UIEndpoint, swaggerOptions.Description);
                option.RoutePrefix = string.Empty;
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
