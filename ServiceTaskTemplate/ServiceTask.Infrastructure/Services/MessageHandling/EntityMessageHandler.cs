using System;
using System.Threading.Tasks;
using Serilog;
using $ext_projectname$.Domain.Models.Messages;
using $ext_projectname$.Infrastructure.Infrastructure.Attributes;
using $ext_projectname$.Infrastructure.Interfaces.QueueService;
using $ext_projectname$.Infrastructure.Interfaces.WorkService;
using UNC.Services;
using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Services.MessageHandling
{
    [Queue(Domain.Constants.QueueConstants.DUMMY_TASK)]
    public class EntityMessageHandler:ServiceBase, IEntityMessageHandler
    {
        private readonly IWorkService _workService;

        public EntityMessageHandler(ILogger logger, IWorkService workService) : base(logger)
        {
            _workService = workService;
        }

        //[Task(Domain.Constants.TaskConstants.INITIALIZE)]
        public async Task<IResponse> HandleRequest(EntityMessage message, Action ackMessage)
        {
            try
            {
                LogBeginRequest();

                await _workService.DoWork(message);

                ackMessage();

                return SuccessResponse();
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


      
    }
}
