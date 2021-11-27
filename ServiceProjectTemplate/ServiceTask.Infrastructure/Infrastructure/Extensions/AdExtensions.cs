using System.Linq;
using $ext_projectname$.Domain.Models.WebServices.ActiveDirectory;
using UNC.Extensions.General;

namespace $ext_projectname$.Infrastructure.Infrastructure.Extensions
{
    public static class AdExtensions
    {
        public static string GetUserPrincipalName(this AdUserModel adUser)
        {
            if (adUser.ProxyAddresses is null || !adUser.ProxyAddresses.Any()) return string.Empty;
            var userPrincipalName = adUser.ProxyAddresses.FirstOrDefault(c => c.EndsWithIgnoreCase("@ad.unc.edu") || c.EndsWithIgnoreCase("@adtest.unc.edu"));

            return userPrincipalName?.Substring(5) ?? "";
        }

        public static string GetPrimaryEmail(this AdUserModel adUser)
        {
            if (adUser.ProxyAddresses is null || !adUser.ProxyAddresses.Any()) return string.Empty;
            var primaryEmail = adUser.ProxyAddresses.Single(c => c.StartsWith("SMTP:"));

            return primaryEmail.Substring(5);
        }
    }
}