using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using $safeprojectname$.Infrastructure;
using UNC.API.Base.Filters;
using UNC.API.Base.Infrastructure;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

namespace $safeprojectname$
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Template.Infrastructure.Services.Mapper.MapperService.RegisterMappings();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {

                services.RegisterDependencies(Configuration);


                SecureApi(services);

                RegisterSwaggerDoc(services);

                
                Log.Information("Startup::ConfigureServices");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private void SecureApi(IServiceCollection services)
        {
            var authority = Configuration.GetValue<string>("IdentityConnection:IdentityServer");
            var requireHttpsMetadata = Configuration.GetValue<bool>("IdentityConnection:RequireHttpsMetadata");
            var skipAuth = Configuration.GetValue<bool>("IdentityConnection:SkipAuth");
            


            //If we are running locally we may not want to deal with authentication, this flag will skip the auth requirements, doesn't 
            if (skipAuth)
            {
                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Write", policy =>
                    {
                        policy.RequireAssertion(context => true);
                    });
                    options.AddPolicy("Read", policy =>
                    {
                        policy.RequireAssertion(context => true);
                    });

                });

                services.AddControllers().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            }
            else
            {
                services.AddAuthentication("Bearer")
                    .AddJwtBearer("Bearer", options =>
                    {
                        options.Authority = authority;
                        options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
                        options.RequireHttpsMetadata = requireHttpsMetadata;
                    });

                services.AddAuthorization(options =>
                {
                    options.AddPolicy("Write", policy =>
                    {
                        policy.RequireAssertion(context => context.User.HasClaim(clm => clm.Type == "scope" && clm.Value == "AD_RW"));
                    });
                    options.AddPolicy("Read", policy =>
                    {
                        policy.RequireAssertion(context => context.User.HasClaim(clm => clm.Type == "scope" && (clm.Value == "AD_RW" || clm.Value == "AD_R")));
                    });

                });

                services.AddControllers(
                        opt =>
                        {
                            //Custom filters can be added here 
                            //opt.Filters.Add(typeof(CustomFilterAttribute));
                            opt.Conventions.Add(new UNC.API.Base.Security.AthorizationControllerConvention());
                            opt.Conventions.Add(new UNC.API.Base.Security.AthorizationActionConvention());
                        })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            }
        }

        private void RegisterSwaggerDoc(IServiceCollection services)
        {
            var appTitle = Configuration.GetValue<string>("Application");

            //AddSwaggerGen is needed to expose authorization button in swagger
            services.AddSwaggerGen(options =>
            {

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = appTitle,
                    Version = "v2"

                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter into field the word 'Bearer' along with a space and JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {new OpenApiSecurityScheme {Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "Bearer"}},Array.Empty<string>()  }

                });

                
                options.ParameterFilter<SwaggerExcludeParameterFilter>();
                options.SchemaFilter<SwaggerExcludeSchemaFilter>();
                options.OperationFilter<SwaggerExcludeOperationFilter>();
                options.AddEnumsWithValuesFixFilters();
            });
            services.AddSwaggerGenNewtonsoftSupport(); // explicit opt-in - needs to be placed after AddSwaggerGen()

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            
            app.UseUncInitializeAutoMapper();


            app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("contacts/swagger.json", "Contacts Controller");
                //options.SwaggerEndpoint("entities/swagger.json", "Entities Controller");
            });

            
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
