using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
namespace WebSiteProject
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", // 路由名稱
            "{controller}/{action}/{id}", // URL 及參數
            new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // 參數預設值
             new string[] { "WebSiteProject.Controllers" } //指定了命名空間
            );

        }
    }
}
