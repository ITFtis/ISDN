using System.Web.Mvc;

namespace WebSiteProject.Areas.webadmin
{
    public class webadminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "webadmin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "webadmin_default",
                "webadmin/{controller}/{action}/{id}",
                new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}