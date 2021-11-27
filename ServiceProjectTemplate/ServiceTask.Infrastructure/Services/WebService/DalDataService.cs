using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailProvisioner.Infrastructure.Interfaces.WebServices;
using Serilog;
using $ext_projectname$.Domain.Models.WebServices.DalData;
using $ext_projectname$.Infrastructure.Interfaces.WebServices;
using UNC.Extensions.General;
using UNC.HttpClient.Interfaces;
using UNC.Services;
using UNC.Services.Interfaces.Response;
using UNC.Services.Responses;

namespace $ext_projectname$.Infrastructure.Services.WebService
{
    public class DalDataService : ServiceBase, IDalDataService, IProcessHistoryLogger
    {
        private readonly IWebClient _endPoint;

        public DalDataService(ILogger logger, IWebClient endPoint, IApiResources apiResources) : base(logger)
        {
            _endPoint = endPoint;
            _endPoint.BaseAddress = apiResources.Resources.Single(c => c.Name.EqualsIgnoreCase("DAL_DATA")).Address;
        }

        public async Task<IResponse> GetAppSettings()
        {
            try
            {
                LogBeginRequest();

                var appDomains = new List<string> { "Office365", "Exchange", "MessageQueue", "Provisioner" };
                var request = await _endPoint.GetEntity<List<AppSettingModel>>("utilities-db/AppSettings");
                var relevantSettings = request.Where(c => appDomains.Any(d => d.EqualsIgnoreCase(c.AppDomain)));
                return CollectionResponse(relevantSettings);
            }
            catch (Exception ex)
            {
                return LogException(ex, false);
            }
            finally
            {
                LogEndRequest();
            }
        }
        

        public async Task<int> LogProcessHistoryStart(string processName, string arguments = "")
        {
            try
            {

                LogBeginRequest();

                var request = await _endPoint.Post<ProcessHistoryModel>("utilities-db/ProcessHistories",
                    new
                    {
                        Name = processName,
                        StartDate = DateTime.Now,
                        Source = "Mail Provisioner",
                        Environment.MachineName,
                        Arguments = arguments
                    });

                return request.Id;
            }
            catch (Exception ex)
            {
                LogException(ex, true);
            }
            finally
            {
                LogEndRequest();
            }

            return -1;

        }

        public async Task LogProcessHistoryCompleted(int processHistoryId)
        {
            try
            {
                LogBeginRequest();
                if (processHistoryId > -1)
                {
                    var pagedResponse = await _endPoint.GetEntity<PagedResponse<ProcessHistoryModel>>($"utilities-db/ProcessHistories?Id={processHistoryId}");
                    var process = pagedResponse.Entities.FirstOrDefault();
                    if (process is null)
                    {
                        LogWarning($"Unable to retrieve process with process id {processHistoryId}");
                        return;
                    }
                    process.EndDate = DateTime.Now;

                    await _endPoint.Put($"utilities-db/ProcessHistories/{processHistoryId}", process);
                }


            }
            catch (Exception ex)
            {
                LogException(ex, true);
            }
            finally
            {
                LogEndRequest();
            }

        }

        public async Task LogProcessHistoryFailed(int processHistoryId, Exception exception)
        {
            try
            {
                LogBeginRequest();
                if (processHistoryId > -1)
                {
                    var pagedResponse = await _endPoint.GetEntity<PagedResponse<ProcessHistoryModel>>($"utilities-db/ProcessHistories?Id={processHistoryId}");
                    var process = pagedResponse.Entities.FirstOrDefault();
                    if (process is null)
                    {
                        LogWarning($"Unable to retrieve process with process id {processHistoryId}");
                        return;
                    }
                    process.EndDate = DateTime.Now;
                    process.Failed = true;
                    process.ErrorMessage = exception.Message;
                    await _endPoint.Put($"utilities-db/ProcessHistories/{processHistoryId}", process);
                }
            }
            catch (Exception ex)
            {
                LogException(ex, true);
            }
            finally
            {
                LogEndRequest();
            }

        }

        public async Task LogProcessHistoryFailed(int processHistoryId, string message)
        {
            try
            {
                LogBeginRequest();
                if (processHistoryId > -1)
                {
                    var pagedResponse = await _endPoint.GetEntity<PagedResponse<ProcessHistoryModel>>($"utilities-db/ProcessHistories?Id={processHistoryId}");
                    var process = pagedResponse.Entities.FirstOrDefault();
                    if (process is null)
                    {
                        LogWarning($"Unable to retrieve process with process id {processHistoryId}");
                        return;
                    }
                    process.EndDate = DateTime.Now;
                    process.Failed = true;
                    process.ErrorMessage = message;
                    await _endPoint.Put($"utilities-db/ProcessHistories/{processHistoryId}", process);
                }
            }
            catch (Exception ex)
            {
                LogException(ex, true);
            }
            finally
            {
                LogEndRequest();
            }

        }

        public async Task LogProcessHistoryDelayed(int processHistoryId, int retryMinutes)
        {
            try
            {
                LogBeginRequest();
                if (processHistoryId > -1)
                {
                    var pagedResponse = await _endPoint.GetEntity<PagedResponse<ProcessHistoryModel>>($"utilities-db/ProcessHistories?Id={processHistoryId}");
                    var process = pagedResponse.Entities.FirstOrDefault();
                    if (process is null)
                    {
                        LogWarning($"Unable to retrieve process with process id {processHistoryId}");
                        return;
                    }

                    process.EndDate = DateTime.Now;
                    process.ErrorMessage = $"Delayed {retryMinutes} minutes";
                    await _endPoint.Put($"ProcessHistories/{processHistoryId}", process);
                }
            }
            catch (Exception ex)
            {
                LogException(ex, true);
            }
            finally
            {
                LogEndRequest();
            }
        }
    }
}
