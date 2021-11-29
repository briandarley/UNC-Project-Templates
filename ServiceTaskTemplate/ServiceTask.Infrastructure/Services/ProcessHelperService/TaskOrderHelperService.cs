using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using $ext_projectname$.Infrastructure.Interfaces.ProcessHelperService;
using $ext_projectname$.Infrastructure.Pocos.ProcessHelperService;
using UNC.Extensions.General;
using UNC.Services;

namespace $ext_projectname$.Infrastructure.Services.ProcessHelperService
{
    public class TaskOrderHelperService : ServiceBase, ITaskOrderHelperService
    {
        private readonly IServiceProvider _serviceProvider;

        public TaskOrderHelperService(ILogger logger, IServiceProvider serviceProvider) : base(logger)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<TaskOrder> GetOrderedTasks()
        {
            yield return new TaskOrder
            {
                Order = 1,
                TaskName = Domain.Constants.TaskConstants.INITIALIZE,
                //ProvisionRequestTask = _serviceProvider.GetService<IInitializeProvisionRequestTask>()
            };
            yield return new TaskOrder
            {
                Order = 2,
                //TaskName = Domain.Constants.TaskConstants.ENABLE_RMT_MAILBOX,
                //ProvisionRequestTask = _serviceProvider.GetService<IEnableRemoteMailboxTask>()
            };
            //...

            //...

            //...
        }

        public TaskOrder GetTaskGivenTaskName(string taskName)
        {
            var allTasks = GetOrderedTasks().OrderBy(c => c.Order).ToList();
            var task = allTasks.SingleOrDefault(c => c.TaskName.EqualsIgnoreCase(taskName));

            if (task is null)
            {
                return allTasks.First();
            }

            return task;
        }

        public TaskOrder GetNextTaskGivenCurrentTask(string taskName)
        {
            var task = GetTaskGivenTaskName(taskName);

            task = GetOrderedTasks().OrderBy(c => c.Order).FirstOrDefault(c => c.Order > task.Order);

            return task;
        }
    }
}
