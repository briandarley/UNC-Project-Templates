using System.Threading.Tasks;
using $ext_projectname$.Domain.Models.WebServices.ActiveDirectory;
using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Interfaces.WebServices
{
    public interface INotificationService
    {
        Task<IResponse> SendOwnerNotification(string principalEmail, string alternativeEmail = "");
        Task<IResponse> SendAdminNotification(AdUserModel adUser);
    }
}