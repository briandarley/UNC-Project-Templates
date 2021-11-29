using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using MailProvisioner.Infrastructure.Interfaces.WebServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Serilog;
using $ext_projectname$.Domain.Constants;
using $ext_projectname$.Domain.Models.WebServices.DalData;
using $ext_projectname$.Domain.Models.WebServices.MessageQueue;
using $ext_projectname$.Domain.Types;
using $ext_projectname$.Infrastructure.Interfaces.ProcessHelperService;
using $ext_projectname$.Infrastructure.Interfaces.WebServices;
using $ext_projectname$.Infrastructure.Services.ProcessHelperService;
using $ext_projectname$.Infrastructure.Services.WebService;
using UNC.Extensions.General;
using UNC.HttpClient;
using UNC.HttpClient.Interfaces;
using UNC.HttpClient.Models;
using UNC.Services;
using UNC.Services.Infrastructure;
using UNC.Services.Interfaces.Response;
using MessageModel = $ext_projectname$.Domain.Models.WebServices.MessageQueue.MessageModel;

namespace $ext_projectname$.ClientHost.Infrastructure
{
    public static class Extensions
    {
        public static void AddDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            UNC.HttpClient.Extensions.Extensions.RegisterDependencies(services);
            services.RegisterCustomLogging(configuration, LogTypes.ConsoleLogging | LogTypes.FileLogging | LogTypes.RemoteApiLogging);

            RegisterInfrastructureServices(services);
            RegisterProvisionTasks(services);
            RegisterProcessHelperServices(services);
            RegisterWorkService(services);
            RegisterRabbitMq(services);
            RegisterRabbitMqDefaultQueueModel(services);
            RegisterRabbitMqConnectionInfo(services);
            RegisterMailTemplates(services);
            RegisterAppSettings(services);
            
            RegisterWebClient(services);
            RegisterWebResourceEndpoints(services);
            

        }
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) => services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
        {
            var host = hostBuilder.UseServiceBaseLifetime().Build();
            var config = host.Services.GetService<IConfiguration>();
            var logger = host.Services.GetService<ILogger>();
            var environment = config.GetSection("Environment").Value;
            logger.Warning($"Running Environment {environment}");


            return host.RunAsync(cancellationToken);
        }

        #region DI Registration Details

        private static void RegisterInfrastructureServices(IServiceCollection services)
        {
            services.AddSingleton<IDalDataService, DalDataService>();
            services.AddSingleton<ILdapService, LdapService>();
            services.AddSingleton<IActiveDirectoryService, ActiveDirectoryService>();
            services.AddSingleton<IProcessHistoryLogger, DalDataService>();
            services.AddSingleton<INotificationService, NotificationService>();
            

            //services.AddHostedService<MailProvisionWatchQueueService>();
        }
        private static void RegisterProvisionTasks(IServiceCollection services)
        {
            //services.AddSingleton<IInitializeProvisionRequestTask, InitializeProvisionRequestTask>();
            //services.AddSingleton<IEnableRemoteMailboxTask, EnableRemoteMailboxTask>();
            //services.AddSingleton<IAddRemoteMailboxAliasesTask, AddRemoteMailboxAliasesTask>();
            //services.AddSingleton<IClearSyncErrorsTask, ClearSyncErrorsTask>();
            //services.AddSingleton<IVerifyMicrosoft365AccountTask, VerifyMicrosoft365AccountTask>();
            //services.AddSingleton<ISetAccountLocationTask, SetAccountLocationTask>();
            //services.AddSingleton<IAddLicensingTask, AddLicensingTask>();
            //services.AddSingleton<IVerifyMailboxProvisionedTask, VerifyMailboxProvisionedTask>();
            //services.AddSingleton<IInitializeOneDriveProvisioningTask, InitializeOneDriveProvisioningTask>();
            //services.AddSingleton<IVerifyAccountSettingsTask, VerifyAccountSettingsTask>();
            //services.AddSingleton<INotifyProvisioningCompleteTask, NotifyProvisioningCompleteTask>();
            //services.AddSingleton<IUpdatePeopleSoftTask, UpdatePeopleSoftTask>();
            //services.AddSingleton<IFinalizeProvisioningRequestTask, FinalizeProvisioningRequestTask>();
            //services.AddSingleton<IVerifyX500PresentTask, VerifyX500PresentTask>();
            //services.AddSingleton<IToggleEmailAddressPolicyTask, ToggleEmailAddressPolicyTask>();


        }
        private static void RegisterProcessHelperServices(IServiceCollection services)
        {
            services.AddSingleton<ITaskOrderHelperService, TaskOrderHelperService>();
            //services.AddSingleton<IProvisionRequestHelperService, ProvisionRequestHelperService>();
            //services.AddSingleton<IAliasCreateService, AliasCreateService>();
        }
        private static void RegisterWorkService(IServiceCollection services)
        {
            //services.AddSingleton<IMailProvisionWorkService, MailProvisionWorkService>();
        }
        private static void RegisterRabbitMq(IServiceCollection services)
        {
            services.AddSingleton(cfg =>
            {
                var logger = cfg.GetService<ILogger>();
                try
                {logger.Warning("Initialize RabbitMq Config");
                    var configuration = cfg.GetService<IConfiguration>();
                    var useLocalQueue = configuration.GetValue<bool>("UseLocalQueue");

                    logger.Warning($"Use Local {useLocalQueue}");
                    var connectionInfo = useLocalQueue 
                        ? configuration.GetSection("LocalQueueConnection").Get<SettingsModel>()
                        : cfg.GetService<SettingsModel>();

                    if (connectionInfo is null)
                    {
                        var lgr = cfg.GetService<ILogger>();
                        lgr.Error("Failed to RegisterRabbitMq in startup configuration, connection info is null");
                    }

                    var connectionFactory = new ConnectionFactory
                    {
                        UserName = connectionInfo.UserName,
                        Password = connectionInfo.Password,
                        HostName = connectionInfo.HostName,
                        AutomaticRecoveryEnabled = true
                        //RequestedConnectionTimeout = TimeSpan.FromSeconds(60),
                        //RequestedHeartbeat = TimeSpan.FromSeconds(60)
                    };

                    

                    if (connectionInfo.Port.HasValue)
                    {
                        connectionFactory.Port = connectionInfo.Port.Value;
                    }

                    if (!string.IsNullOrEmpty(connectionInfo.VirtualHost))
                    {
                        connectionFactory.VirtualHost = connectionInfo.VirtualHost;
                    }

                    if (connectionInfo.UseSsl.HasValue && connectionInfo.UseSsl.Value)
                    {
                        connectionFactory.Ssl.ServerName = connectionInfo.HostName;
                        connectionFactory.Ssl.Enabled = true;
                        connectionFactory.Ssl.Version = (SslProtocols)Enum.Parse(typeof(SslProtocols), connectionInfo.SslVersion);

                    }
                    

                    return connectionFactory;
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "An error occured while configuring RabbitMQ connection");
                    throw;
                }

            });
        }
        private static void RegisterRabbitMqDefaultQueueModel(IServiceCollection services)
        {
            services.AddSingleton(cfg =>
            {
                //var queueModel = new QueueModel
                //{
                //    ExchangeName = ExchangeConstants.PROVISION_EXCHANGE,
                //    QueueName = QueueConstants.MAIL_PROVISION_TASK,
                //    QueueType = QueueTypes.Classic
                //};
                //return queueModel;
                //TODO setup default queue
                return new object();
            });
        }
        private static void RegisterRabbitMqConnectionInfo(IServiceCollection services)
        {
            services.AddSingleton(cfg =>
            {
                var logService = cfg.GetService<ILogService>();
                var appSettings = cfg.GetService<List<AppSettingModel>>();

                var queueSettings = appSettings.SingleOrDefault(c =>
                    c.AppDomain.EqualsIgnoreCase("MessageQueue")
                    && c.Name.EqualsIgnoreCase("SelfServiceMessageQue"));
                if (queueSettings is null)
                {
                    logService.LogError("Failed to retrieve settings for MessageQueue");
                    throw new Exception("Failed to retrieve MessageQueue settings");
                }
                var connectionInfo = queueSettings.Value.FromJson<SettingsModel>();

                return connectionInfo;
            });
        }
        private static void RegisterMailTemplates(IServiceCollection services)
        {
            services.AddSingleton(cfg =>
            {
                //Can't use (If we are running as a service the directory referenced will be c:\windows\system32 not what we want
                //Environment.CurrentDirectory

                var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                path = Path.GetDirectoryName(path);
                
                path = Path.Combine(path, @"MailTemplates\Template.MailCreationNotification.html");
                var fileContents = File.ReadAllText(path);

                var appSettings = cfg.GetService<List<AppSettingModel>>();
                var messageModel = JsonConvert.DeserializeObject<MessageModel>(
                    appSettings.Single(c => c.Name.Equals("Notification", StringComparison.CurrentCultureIgnoreCase)
                                            && c.AppDomain.Equals("Provisioner", StringComparison.CurrentCultureIgnoreCase)).Value);


                messageModel.Body = fileContents;

                return messageModel;

            });
        }
        private static void RegisterAppSettings(IServiceCollection services)
        {
            services.AddSingleton(cfg =>
            {
                var dalData = cfg.GetService<IDalDataService>();
                var request = dalData.GetAppSettings();
                request.Wait();
                var appSettings = ((ICollectionResponse<AppSettingModel>)request.Result).Entities.ToList();
                return appSettings;
            });
        }
        
        private static void RegisterWebClient(IServiceCollection services)
        {
            services.AddTransient<IWebClient>(cfg =>
            {
                var configuration = cfg.GetService<IConfiguration>();

                var clientId = configuration.GetValue<string>("ClientId");
                var application = configuration.GetValue<string>("Application");

                var authSettings = configuration.GetSection("IdentityConnection").Get<IdentityConnection>();


                var client = new WebClient(cfg.GetService<ILogger>(), clientId, application, authSettings, cfg.GetService<IPrincipal>());
                client.TokenRefreshed += (sender, args) =>
                {


                };
                return client;
            });
        }
        private static void RegisterWebResourceEndpoints(IServiceCollection services)
        {
            services.AddSingleton(cfg =>
            {
                var configuration = cfg.GetService<IConfiguration>();
                var node = configuration.GetSection("Endpoints").GetChildren();
                var appResources = (IApiResources)new ApiResources();
                foreach (var section in node)
                {
                    var resource = new ApiResource { Address = section.Value, Name = section.Key };
                    appResources.Resources.Add(resource);
                }

                return appResources;

            });
        }
        

        #endregion

    }
}
