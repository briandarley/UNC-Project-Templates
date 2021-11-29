using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;
using Template.Infrastructure.Interfaces.Services;
using Template.Infrastructure.Pocos;
using UNC.Extensions.General;
using UNC.HttpClient.Interfaces;
using UNC.Services;
using UNC.Services.Interfaces.Response;

namespace Template.Infrastructure.Services.Api
{
    public class DalDataService : ServiceBase, IDalDataService
    {
        private readonly IWebClient _endPoint;

        public DalDataService(ILogger logger, IApiResources apiResources, IWebClient endPoint) : base(logger)
        {
            _endPoint = endPoint;
            _endPoint.BaseAddress = apiResources.GetAddress("DAL_DATA");
        }

        

        public async Task<IResponse> GetSettings()
        {
            try
            {
                LogBeginRequest();

                var rawRequest = await _endPoint.GetRaw($"utilities-db/AppSettings");

                var entities = rawRequest.FromJson<List<Setting>>();
                

                return CollectionResponse(entities);


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
