using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using UNC.Extensions.General;

namespace Template.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args);

            hostBuilder
                
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());

                    config.AddJsonFile("appsettings.json", false, true);

                    var environment = config.Build().GetValue<string>("Environment");
                    if (environment.IsEmpty())
                    {
                        environment = Environment.GetEnvironmentVariable("ENVIRONMENT");
                    }
                    config
                        .AddJsonFile("config.json", optional: false, reloadOnChange: true);
                    config
                        .AddJsonFile($"config.{environment}.json", optional: false, reloadOnChange: true);

                    config.AddEnvironmentVariables();

                });



            var webHost = hostBuilder.Build();
            try
            {
                Log.Information("Starting web host");
                webHost.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
