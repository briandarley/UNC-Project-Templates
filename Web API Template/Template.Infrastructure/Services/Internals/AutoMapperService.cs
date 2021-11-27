using AutoMapper;
using UNC.Services.Infrastructure;

namespace $safeprojectname$.Services.Internals
{
    public class AutoMapperService : AutoMapperServiceBase
    {
        public AutoMapperService()
        {
            RegisterMappings();
        }

        protected override IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg => { });
            return config.CreateMapper();
        }
    }
}
