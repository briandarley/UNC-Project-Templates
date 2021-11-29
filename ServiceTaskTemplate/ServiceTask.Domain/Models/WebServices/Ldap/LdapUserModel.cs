using System.Collections.Generic;
using UNC.Services.Interfaces;

namespace $ext_projectname$.Domain.Models.WebServices.Ldap
{
    public class LdapUserModel:IEntity
    {
        public string Pid { get; set; }
        public string Uid { get; set; }
        public string Mail { get; set; }
        public bool IsActive { get; set; }
        public List<string> UncEmail { get; set; }
        public string GivenName { get; set; }
        public string Sn { get; set; }
        public string MiddleName { get; set; }
        public List<string> EduPersonAffiliation { get; set; }
        public List<string> EmployeeType { get; set; }
        public List<string> UncAffiliateType { get; set; }
        public List<string> EduPersonScopedAffiliation { get; set; }
        public List<string> UncStudentType { get; set; }
    }
}
