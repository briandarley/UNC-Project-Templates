using System;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using $ext_projectname$.Domain.Criteria.Ldap;
using $ext_projectname$.Domain.Models.WebServices.Ldap;
using $ext_projectname$.Infrastructure.Interfaces.WebServices;
using UNC.Extensions.General;
using UNC.HttpClient.Interfaces;
using UNC.Services;
using UNC.Services.Interfaces.Response;
using UNC.Services.Responses;

namespace $ext_projectname$.Infrastructure.Services.WebService
{
    public class LdapService:ServiceBase, ILdapService
    {
        private readonly IWebClient _endPoint;

        public LdapService(ILogger logger, IWebClient endPoint, IApiResources apiResources) : base(logger)
        {
            _endPoint = endPoint;
            _endPoint.BaseAddress = apiResources.Resources.Single(c => c.Name.EqualsIgnoreCase("LDAP")).Address;
        }

        public async Task<IResponse> GetLdapUser(string identity)
        {
            try
            {
                LogBeginRequest();

                var pagedResponse = await _endPoint.GetEntity<PagedResponse<LdapUserModel>>($"users?{new { Uid = identity }.ToQueryParams()} ");
                
                if (pagedResponse.TotalRecords == 0)
                {
                    return NotFoundResponse();
                }

                return EntityResponse(pagedResponse.Entities.Single());

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

        public async Task<IResponse> GetLdapUsers(LdapUserCriteria criteria)
        {
            try
            {
                LogBeginRequest();

                var pagedResponse = await _endPoint.GetEntity<PagedResponse<LdapUserModel>>($"users?{criteria.ToQueryParams()}");

               
                return pagedResponse;

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
