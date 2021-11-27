using System.Collections.Generic;

namespace $ext_projectname$.Domain.Models.WebServices.ActiveDirectory
{
    public class AdEntityModel
    {
        public string SamAccountName { get; set; }
        public List<string> ProxyAddresses { get; set; }
    }
}
