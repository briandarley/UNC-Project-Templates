using UNC.Services.Criteria;

namespace $ext_projectname$.Domain.Criteria.ActiveDirectory
{
    public class EntityCriteria: BasePagingCriteria
    {
        public string RawQuery { get; set; }
        public string ProxyAddress { get; set; }
    }
}
