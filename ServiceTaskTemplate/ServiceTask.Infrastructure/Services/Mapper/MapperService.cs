using System;
using AutoMapper;

namespace $ext_projectname$.Infrastructure.Services.Mapper
{

    public class MapperService
    {
        public static IMapper Mapper;

        public static void RegisterMappings()
        {
            if (Mapper != null) return;

            Mapper = GetMapper();
        }

        private static IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //UtilityDbMapperService.RegisterMappings(cfg);

            });

            try
            {
                config.AssertConfigurationIsValid();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return config.CreateMapper();
        }
    }

}