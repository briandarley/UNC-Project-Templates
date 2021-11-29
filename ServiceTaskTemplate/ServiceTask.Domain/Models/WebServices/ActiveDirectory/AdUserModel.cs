using System.Collections.Generic;
using UNC.Services.Interfaces;

namespace $ext_projectname$.Domain.Models.WebServices.ActiveDirectory
{
    public class AdUserModel : IEntity
    {

        public string SamAccountName { get; set; }
        public bool AccountEnabled { get; set; }
        public bool Enabled { get; set; }
        public string DistinguishedName { get; set; }
        public string Division { get; set; }
        public string EmployeeId { get; set; }
        public bool MsRtcsipEnabled { get; set; }
        public string MsRtcsipPrimaryUserAddress { get; set; }
        public string MsExchGenericForwardingAddress { get; set; }
        public List<string> ProxyAddresses { get; set; }
        public List<string> ExtensionAttributes { get; set; }
        public string GivenName { get; set; }
        public string Sn { get; set; }
        public List<string> DepartmentNumber { get; set; }
        public string Initials { get; set; }
    }
}
