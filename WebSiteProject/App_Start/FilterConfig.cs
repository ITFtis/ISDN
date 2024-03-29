﻿using System;
using System.Web;
using System.Web.Mvc;

namespace WebSiteProject
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new NoCacheAttribute());
        }
        public class NoCacheAttribute : ActionFilterAttribute
        {
            public override void OnResultExecuting(ResultExecutingContext filterContext)
            {
                filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                filterContext.HttpContext.Response.Cache.SetExpires(DateTime.MinValue);
                filterContext.HttpContext.Response.Cache.SetNoStore();

                //filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                //filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
                //filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
                //filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                //filterContext.HttpContext.Response.Cache.SetNoStore();
                base.OnResultExecuting(filterContext);
            }
        }
    }
}
