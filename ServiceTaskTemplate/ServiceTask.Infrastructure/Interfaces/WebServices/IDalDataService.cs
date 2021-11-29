using System.Threading.Tasks;
using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Interfaces.WebServices
{
    public interface IDalDataService
    {
        Task<IResponse> GetAppSettings();
        
    }
}