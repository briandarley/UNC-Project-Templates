using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using $ext_projectname$.Infrastructure.Interfaces.ProcessHelperService;
using TaskNames = $ext_projectname$.Domain.Constants.TaskConstants;
namespace $ext_projectname$.ClientHost
{
    public class ConsoleHostedService
    {

        public async Task RunConsole(string[] args)
        {
            //await RunSpecificTask();
            await RunWorkerProcess();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private async Task RunWorkerProcess()
        {
            var args = new[] { "-attachQueue", "-runConsole" };
            await Program.InitializeHost(args);
        }

        private async Task RunSpecificTask()
        {
            await Program.InitializeHost(null);

            //var args = new[] { "-attachQueue" };
            var services = Program.Builder.Build().Services;

            var taskHelper = services.GetService<ITaskOrderHelperService>();

            var taskOrder = taskHelper.GetTaskGivenTaskName(TaskNames.INITIALIZE);
            
            //var request = await taskOrder.ProvisionRequestTask.ProcessTask(new ProvisionMailboxMessage());



        }

    }
}
