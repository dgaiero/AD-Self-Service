using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Security.Claims;
using System.Threading;
using System.Security.Principal;

namespace WebApplication1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = System.Security.Claims.ClaimTypes.Upn;
        }
    }
}

public static class UserExtended
{
    public static string GetGivenName(this IPrincipal user)
    {
        var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Name);
        return claim == null ? null : claim.Value;
    }
    public static string GetUPN(this IPrincipal user)
    {
        var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Upn);
        return claim == null ? null : claim.Value;
    }
    public static string GetGroup(this IPrincipal user)
    {
        var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.WindowsDeviceGroup);
        return claim == null ? null : claim.Value;
    }
    public static string GetEmailAddress(this IPrincipal user)
    {
        var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.Email);
        return claim == null ? null : claim.Value;
    }
    public static string GetsAMAccountID(this IPrincipal user)
    {
        var claim = ((ClaimsIdentity)user.Identity).FindFirst(ClaimTypes.GivenName);
        return claim == null ? null : claim.Value;
    }

}