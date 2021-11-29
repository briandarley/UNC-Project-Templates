using System;
using System.Linq;
using $ext_projectname$.Domain.Models.AppSettings;
using Microsoft.Extensions.Configuration;


namespace $ext_projectname$.ClientHost
{
    public class WindowsHostedService : BackgroundService
    {
        public WindowsHostedService(IConfiguration configuration, Arguments arguments, IServiceProvider serviceProvider) : base(configuration)
        {
            if (arguments.AppArguments.Any(c => c.Equals("-console")))
            {

            }

        }
    }
}
