using System;
using System.Threading.Tasks;

namespace MailProvisioner.Infrastructure.Interfaces.WebServices
{
    public interface IProcessHistoryLogger
    {
        Task<int> LogProcessHistoryStart(string processName, string arguments = "");
        Task LogProcessHistoryCompleted(int processHistoryId);
        Task LogProcessHistoryFailed(int processHistoryId, Exception exception);
        Task LogProcessHistoryFailed(int processHistoryId, string message);
        Task LogProcessHistoryDelayed(int processHistoryId, int retryMinutes);
    }
}
