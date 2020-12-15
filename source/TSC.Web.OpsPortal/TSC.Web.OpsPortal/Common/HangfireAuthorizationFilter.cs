using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;

namespace TSC.Web.OpsPortal.Common
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        /// <summary>
        /// Parameter to specify the configuration appsetting that contains the allowed GGs
        /// </summary>
        public string AllowedGroupNamesConfigKey { get; set; }

        public bool Authorize(DashboardContext context)
        {
            // get current user
            return true;
        }
    }

    
}