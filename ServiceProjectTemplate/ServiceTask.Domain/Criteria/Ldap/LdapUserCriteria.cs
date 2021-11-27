using UNC.Services.Criteria;

namespace $ext_projectname$.Domain.Criteria.Ldap
{
    public class LdapUserCriteria: BasePagingCriteria
    {
        public string Email { get; set; }
    }
}
