using System.Threading.Tasks;
using UNC.Services.Interfaces.Response;

namespace Template.Infrastructure.Interfaces.Services
{
    public interface IDalDataService
    {
        Task<IResponse> GetSettings();
    }
}
