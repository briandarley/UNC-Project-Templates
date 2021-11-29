using System.Linq;
using System.Security.Principal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Template.Infrastructure.Interfaces.Services;
using Template.Infrastructure.Pocos;
using Template.Infrastructure.Services.Internals;
using UNC.API.Base.Infrastructure;
using UNC.HttpClient;
using UNC.HttpClient.Interfaces;
using UNC.HttpClient.Models;
using UNC.Services;
using UNC.Services.Infrastructure;
using UNC.Services.Interfaces.Response;

namespace Template.Api.Infrastructure
{
    public static class Extensions
    {
        
        public static void RegisterDependencies(this IServiceCollection services, IConfiguration configuration)
        {

            UNC.HttpClient.Extensions.Extensions.RegisterDependencies(services);
            Template.Infrastructure.Services.Mapper.MapperService.RegisterMappings();


            RegisterApplicationServices(services);
            RegisterInfrastructureServices(services);
            RegisterRepositories(services);


            
            services.AddSwaggerGen(options =>
            {

                options.SwaggerDoc("contacts", new OpenApiInfo { Title = "Contacts Controller", Version = "v1", Description = "API Endpoints for RWD of Contacts" });
                options.SwaggerDoc("entities", new OpenApiInfo { Title = "Entities Controller", Version = "v1", Description = "API Endpoints for R of Entities" });


            });





            services.RegisterApiVersioning();
            services.RegisterDefaultCors();
            services.RegisterDefaultMvcSerialization();

            services.RegisterApiEndPoints(configuration);
            services.RegisterCustomLogging(configuration, LogTypes.ConsoleLogging | LogTypes.FileLogging | LogTypes.RemoteApiLogging);
            services.RegisterLogHttpClient(configuration);
            services.RegisterAutoMapper<AutoMapperService>();
            services.AddTransient<ILogService, LogService>();


            services.AddHttpContextAccessor();
            
            services.AddSingleton(cfg =>
            {
                var dataService = cfg.GetRequiredService<IDalDataService>();
                var settingsRequest = dataService.GetSettings();
                settingsRequest.Wait();
                var settings = ((ICollectionResponse<Setting>)settingsRequest.Result).Entities;
                return settings.ToList();
            });


            services.AddTransient<IWebClient>(cfg =>
            {
                var clientId = configuration.GetValue<string>("ClientId");
                var application = configuration.GetValue<string>("Application");

                var authSettings = configuration.GetSection("IdentityConnection").Get<IdentityConnection>();


                var client = new WebClient(cfg.GetService<Serilog.ILogger>(), clientId, application, authSettings, cfg.GetService<IPrincipal>());
                client.TokenRefreshed += (sender, args) =>
                {


                };
                return client;
            });


            services.AddSingleton(cfg =>
            {
                var node = configuration.GetSection("Endpoints").GetChildren();
                var appResources = (IApiResources)new UNC.HttpClient.Models.ApiResources();
                foreach (var section in node)
                {
                    var resource = new UNC.HttpClient.Models.ApiResource { Address = section.Value, Name = section.Key };
                    appResources.Resources.Add(resource);
                }

                return appResources;

            });

            
            RegisterInfrastructureDependencies(services);
        }

        private static void RegisterApplicationServices(IServiceCollection services)
        {
            
            services.AddSingleton<UNC.Services.Utilities.Encryption>();
            //services.AddSingleton<Template.Application.Interfaces.IContactService, MediaTypeNames.Application.Services.ContactService>();
            

        }

        private static void RegisterInfrastructureServices(IServiceCollection services)
        {
            services.AddSingleton<Template.Infrastructure.Interfaces.Services.IDalDataService, Template.Infrastructure.Services.Api.DalDataService>();
            
        }
        
        private static void RegisterRepositories(IServiceCollection services)
        {

            
        }


        private static void RegisterInfrastructureDependencies(IServiceCollection services)
        {
            
            

        }

    }
}
