using UNC.Services.Criteria;

namespace $ext_projectname$.Domain.Criteria.ActiveDirectory
{
    public class AdUserCriteria : BasePagingCriteria
    {
        public string SamAccountName { get; set; }

        public string ProxyAddress { get; set; }

        public string EmployeeId { get; set; }

    }
}
