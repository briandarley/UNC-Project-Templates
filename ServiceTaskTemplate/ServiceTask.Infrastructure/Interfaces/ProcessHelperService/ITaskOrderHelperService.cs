using System.Collections.Generic;
using $ext_projectname$.Infrastructure.Pocos.ProcessHelperService;

namespace $ext_projectname$.Infrastructure.Interfaces.ProcessHelperService
{
    public interface ITaskOrderHelperService
    {
        IEnumerable<TaskOrder> GetOrderedTasks();
        TaskOrder GetTaskGivenTaskName(string taskName);
        TaskOrder GetNextTaskGivenCurrentTask(string taskName);
    }
}