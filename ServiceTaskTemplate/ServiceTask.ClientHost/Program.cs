using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using $ext_projectname$.ClientHost.Infrastructure;
using $ext_projectname$.Domain.Models.AppSettings;
using UNC.Extensions.General;

namespace $ext_projectname$.ClientHost
{
    class Program
    {
        public static IHostBuilder Builder;
        public static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("-console"));

            if (isService)
            {
                args = args.Concat(new[] { "-attachQueue", "-runAsService" }).ToArray();

                await InitializeHost(args);

            }
            else
            {
                args = args.Concat(new[] { "-runConsole" }).ToArray();
                var console = new ConsoleHostedService();
                await console.RunConsole(args);
            }

        }


        public static async Task InitializeHost(string[] args)
        {

            Builder = Host.CreateDefaultBuilder(args)
            //Builder = new HostBuilder()
                .UseWindowsService(options =>
                {
                    options.ServiceName = "UNC - Task - Mail Provisioner";
                })
                .ConfigureAppConfiguration((context, config) =>
                {
                    //Can't use (If we are running as a service the directory referenced will be c:\windows\system32 not what we want
                    //config.SetBasePath(Directory.GetCurrentDirectory());
                    //context.Configuration.
                    
                    var path = AppDomain.CurrentDomain.BaseDirectory;
                    path = Path.GetDirectoryName(path);
                    config.SetBasePath(path);
                    

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

                  

                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOptions();
                    services.AddDependencies(context.Configuration);

                    if (args != null && args.Contains("-attachQueue"))
                    {
                        services.AddHostedService<WindowsHostedService>();
                    }


                    var argumentList = new Arguments { AppArguments = new List<string>() };
                    if (args != null && args.Any())
                    {
                        argumentList.AppArguments = args.ToList();
                    }
                    services.AddSingleton(argumentList);


                })
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                    logging.AddConsole();


                });

            if (args != null && args.Contains("-runConsole"))
            {
                await Builder.RunConsoleAsync();
            }
            else if (args != null && args.Contains("-runAsService"))
            {
                await Builder.RunAsServiceAsync();
            }
            //await Builder.RunAsServiceAsync();


        }
    }
}

