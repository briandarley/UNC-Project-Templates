using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace $ext_projectname$.ClientHost
{
    public abstract class BackgroundService : IHostedService
    {
        public IConfiguration Configuration { get; }

        protected BackgroundService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

       
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Running Mail Provisioner");
            

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Exiting Mail Provisioner...");
            return Task.CompletedTask;
        }
    }
}
