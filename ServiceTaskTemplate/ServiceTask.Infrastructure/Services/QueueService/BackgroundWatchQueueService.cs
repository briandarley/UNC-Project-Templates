using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailProvisioner.Domain.Models.WebServices.MessageQueue;
using MailProvisioner.Infrastructure.Infrastructure.Attributes;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using Serilog;
using Serilog.Events;
using UNC.Extensions.General;
using UNC.LogHandler.Models;
using UNC.Services.Interfaces.Response;
using UNC.Services.Responses;

namespace $ext_projectname$.Infrastructure.Services.QueueService
{
    public abstract class BackgroundWatchQueueService : BackgroundService, IDisposable
    {
        private readonly ILogger _logger;
        protected readonly IServiceProvider ServiceProvider;
        protected readonly ConnectionFactory ConnectionFactory;
        protected readonly QueueModel Que;
        protected IConnection Connection;
        protected IModel Channel;

        protected BackgroundWatchQueueService(ILogger logger, IServiceProvider serviceProvider, ConnectionFactory connectionFactory, QueueModel que)
        {
            _logger = logger;
            ServiceProvider = serviceProvider;
            ConnectionFactory = connectionFactory;
            Que = que;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                LogBeginRequest();


                return base.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                LogException(ex, false);
                return base.StartAsync(cancellationToken);
            }
            finally
            {
                LogEndRequest();
            }

        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                LogBeginRequest();

                await base.StopAsync(cancellationToken);
                Connection.Close();
            }
            catch (Exception ex)
            {
                LogException(ex, false);
            }
            finally
            {
                LogEndRequest();
            }
        }


        public static Type GetMessageHandlerForQueue(string queueName)
        {
            var assembly = Assembly.GetAssembly(typeof(BackgroundWatchQueueService));
            var type = assembly.GetTypes().SingleOrDefault(c => c.GetCustomAttribute<QueueAttribute>() != null && c.GetCustomAttribute<QueueAttribute>().QueueName.EqualsIgnoreCase(queueName));
            return type;
        }

        public new void Dispose()
        {
            base.Dispose();
            Connection?.Dispose();
            Channel?.Dispose();

        }


        #region Logging Implementation

        private LogActivityMessage InitializeLogEntry(string callerName, string sourcePath, int lineNumber)
        {



            int? ln = null;
            if (lineNumber > 0)
            {
                ln = lineNumber;
            }
            var logEntry = new LogActivityMessage
            {

                AuthUser = "TASK_MANAGER",
                Method = callerName,
                FilePath = sourcePath,
                LineNumber = ln

            };
            return logEntry;
        }

        protected virtual void LogBeginRequest(
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string sourcePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logEntry = InitializeLogEntry(callerName, sourcePath, sourceLineNumber);
            logEntry.Level = LogEventLevel.Information.ToString();
            logEntry.Message = "Begin Request";

            _logger.Information(logEntry.ToJson());

        }
        protected virtual void LogEndRequest(TimeSpan? elapsed = null, [CallerMemberName] string callerName = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {

            var logEntry = InitializeLogEntry(callerName, sourcePath, sourceLineNumber);
            logEntry.Level = LogEventLevel.Information.ToString();
            logEntry.Message = "End Request";
            logEntry.Elapsed = elapsed;

            _logger.Information(logEntry.ToJson());

        }
        protected IErrorResponse LogWarning(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool includeFullTrace = true)
        {
            var logEntry = InitializeLogEntry(callerName, sourcePath, sourceLineNumber);
            logEntry.Level = LogEventLevel.Warning.ToString();

            var sb = new StringBuilder(message);


            if (includeFullTrace && FullTrace().ToList().Count > 0)
            {
                sb.AppendLine("Full trace follows...");
                sb.AppendLine($"Trace: {string.Join(",", FullTrace().ToList())}");
            }

            logEntry.Message = sb.ToString();

            _logger.Warning(logEntry.ToJson());

            var warningMessage = $"{message}: {callerName}";
            var response = new WarningResponse(warningMessage);

            return response;
        }
        protected IErrorResponse LogError(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool includeFullTrace = true)
        {
            var logEntry = InitializeLogEntry(callerName, sourcePath, sourceLineNumber);
            logEntry.Level = LogEventLevel.Error.ToString();

            var sb = new StringBuilder(message);


            if (includeFullTrace && FullTrace().ToList().Count > 0)
            {
                sb.AppendLine("Full trace follows...");
                sb.AppendLine($"Trace: {string.Join(",", FullTrace().ToList())}");
            }

            logEntry.Message = sb.ToString();

            _logger.Warning(logEntry.ToJson());

            var warningMessage = $"{message}: {callerName}";
            var response = new WarningResponse(warningMessage);

            return response;
        }
        protected IInfoResponse LogInfo(string message, [CallerMemberName] string callerName = "", [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLineNumber = 0, bool includeFullTrace = false)
        {
            var logEntry = InitializeLogEntry(callerName, sourcePath, sourceLineNumber);
            logEntry.Level = LogEventLevel.Information.ToString();

            var sb = new StringBuilder(message);


            if (includeFullTrace && FullTrace().ToList().Count > 0)
            {
                sb.AppendLine("Full trace follows...");
                sb.AppendLine($"Trace: {string.Join(",", FullTrace().ToList())}");
            }

            logEntry.Message = sb.ToString();

            _logger.Warning(logEntry.ToJson());

            var warningMessage = $"{message}: {callerName}";
            var response = new InfoResponse(warningMessage);

            return response;

        }

        protected IDebugResponse LogDebug(string message,
            [CallerMemberName] string callerName = "",
            [CallerFilePath] string sourcePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            bool includeFullTrace = false)
        {
            var logEntry = InitializeLogEntry(callerName, sourcePath, sourceLineNumber);
            logEntry.Level = LogEventLevel.Debug.ToString();

            var sb = new StringBuilder(message);


            if (includeFullTrace && FullTrace().ToList().Count > 0)
            {
                sb.AppendLine("Full trace follows...");
                sb.AppendLine($"Trace: {string.Join(",", FullTrace().ToList())}");
            }

            logEntry.Message = sb.ToString();

            _logger.Debug(logEntry.ToJson());

            var warningMessage = $"{message}: {callerName}";
            var response = new DebugResponse(warningMessage);

            return response;
        }

        protected IExceptionResponse LogException(
            Exception ex,
            bool throwException,
            [CallerMemberName]
            string callerName = "",
            [CallerFilePath] string sourcePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var logEntry = InitializeLogEntry(callerName, sourcePath, sourceLineNumber);
            logEntry.Level = LogEventLevel.Error.ToString();

            var sb = new StringBuilder($"Unexpected Error calling {callerName}");
            sb.Append(ex.Message);

            if (ex.InnerException != null)
            {
                sb.AppendLine("Inner Exception: ");
                sb.AppendLine(ex.InnerException.Message);
            }


            if (FullTrace().ToList().Count > 0)
            {
                sb.Append(" Full trace follows...");
                sb.AppendLine($"Trace: {string.Join(",", FullTrace().ToList())}");
            }

            logEntry.Message = sb.ToString();

            _logger.Error(logEntry.ToJson());

            if (throwException)
            {
                throw ex;
            }


            var errorMessage = $"{ex.Message}: {callerName}";
            return new ExceptionResponse(errorMessage) { Exception = ex };

        }


        private IEnumerable<string> FullTrace()
        {
            var index = 0;
            var stackTrace = new StackTrace(true);

            var frames = stackTrace.GetFrames();
            if (frames == null || !frames.Any())
            {
                yield break;
            }
            foreach (var r in frames)
            {
                if (index == 0)
                {
                    continue;
                }
                index++;
                yield return $"Filename: {r.GetFileName()} Method: {r.GetMethod()} Line: {r.GetFileLineNumber()} Column: {r.GetFileColumnNumber()}  ";
            }

        }

        #endregion
    }
}
