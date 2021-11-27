using System.Threading.Tasks;
using $ext_projectname$.Domain.Criteria.ActiveDirectory;
using $ext_projectname$.Domain.Models.WebServices.ActiveDirectory;
using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Interfaces.WebServices
{
    public interface IActiveDirectoryService
    {
        Task<IResponse> GetAdUser(AdUserCriteria criteria);
        Task<IResponse> GetAdEntities(EntityCriteria criteria);
        Task<IResponse> UpdateProxyAddresses(AdUserModel adUser);
        Task<IResponse> ClearSyncErrors(AdUserModel adUser);
    }
}