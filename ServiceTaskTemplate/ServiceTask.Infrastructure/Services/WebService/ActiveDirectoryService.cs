using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using $ext_projectname$.Domain.Criteria.ActiveDirectory;
using $ext_projectname$.Domain.Models.WebServices.ActiveDirectory;
using $ext_projectname$.Infrastructure.Interfaces.WebServices;
using UNC.Extensions.General;
using UNC.HttpClient.Interfaces;
using UNC.Services;
using UNC.Services.Interfaces.Response;
using UNC.Services.Responses;

namespace $ext_projectname$.Infrastructure.Services.WebService
{
    public class ActiveDirectoryService : ServiceBase, IActiveDirectoryService
    {
        private readonly IWebClient _endPoint;

        public ActiveDirectoryService(ILogger logger, IWebClient endPoint, IApiResources apiResources) : base(logger)
        {
            _endPoint = endPoint;
            _endPoint.BaseAddress = apiResources.Resources.Single(c => c.Name.EqualsIgnoreCase("ACTIVE_DIRECTORY")).Address;
        }

        public async Task<IResponse> GetAdUser(AdUserCriteria criteria)
        {
            try
            {
                LogBeginRequest();

                var request = await _endPoint.GetEntity<PagedResponse<AdUserModel>>($"users?{criteria.ToQueryParams()}");

                if (request.TotalRecords != 1)
                {
                    LogWarning($"User with given criteria {criteria.ToQueryParams()} not found");
                    return NotFoundResponse();
                }

                return EntityResponse(request.Entities.Single());

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

        public async Task<IResponse> GetAdEntities(EntityCriteria criteria)
        {
            try
            {
                LogBeginRequest();

                var request = await _endPoint.GetEntity<PagedResponse<AdEntityModel>>($"entities?{criteria.ToQueryParams()}");

                return request;
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

        public async Task<IResponse> UpdateProxyAddresses(AdUserModel adUser)
        {
            try
            {
                LogBeginRequest();

                if (adUser.DistinguishedName.IsEmptyOrNull())
                {
                    Return LogError("Required property DistinguishedName is required on object AdUser");
                }

                if (adUser.ProxyAddresses is null || !adUser.ProxyAddresses.Any())
                {
                    Return LogError("Required property ProxyAddresses is empty or null");
                }

                var model = new
                {
                    adUser.ProxyAddresses
                };

                await _endPoint.Put($"users/{adUser.DistinguishedName}", model);

                return SuccessResponse();


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

        public async Task<IResponse> ClearSyncErrors(AdUserModel adUser)
        {
            try
            {
                LogBeginRequest();

                if (adUser.DistinguishedName.IsEmptyOrNull())
                {
                    Return LogError("Required property DistinguishedName is required on object AdUser");
                }

                if (adUser.ExtensionAttributes is null)
                {
                    adUser.ExtensionAttributes = new List<string>();
                }

                var model = new
                {
                    adUser.ExtensionAttributes
                };

                await _endPoint.Put($"users/{adUser.DistinguishedName}", model);

                return SuccessResponse();


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
