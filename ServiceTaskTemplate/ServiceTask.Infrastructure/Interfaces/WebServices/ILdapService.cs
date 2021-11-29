using System.Threading.Tasks;
using $ext_projectname$.Domain.Criteria.Ldap;
using UNC.Services.Interfaces.Response;

namespace $ext_projectname$.Infrastructure.Interfaces.WebServices
{
    public interface ILdapService
    {
        Task<IResponse> GetLdapUser(string identity);
        Task<IResponse> GetLdapUsers(LdapUserCriteria criteria);
    }
}