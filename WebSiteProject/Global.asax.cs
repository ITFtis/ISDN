using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebSiteProject
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MvcHandler.DisableMvcResponseHeader = true;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            InitConfig.RegisterLang();
            InitConfig.RegisterSiteData();
            InitConfig.RegisterLangText();
            Application["online"] = 0;
            Application["onlinedate"] = DateTime.Now.ToString("yyyyMMdd");
            Application["todayonline"] = 0;
            Application.UnLock();
        }
        #region 如果需要自訂角色的話
        //void Application_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    if (Request.IsAuthenticated)
        //    {

        //        // 將儲存在 FormsAuthenticationTicket 中的角色定義取出，並轉成字串陣列
        //        string[] roles = new string[] { "ken"};
        //        // 指派角色到目前這個 HttpContext 的 User 物件去
        //        Context.User = new GenericPrincipal(Context.User.Identity, roles);
        //    }
        //}
        #endregion

        protected void Session_End()
        {
            Application.Lock();
            int online = (int)Application["online"];
            if (Session["LoginDate"] != null) {
                Application["online"] = online - 1;
            }
            Application.UnLock();
        }

        #region Application_PreSendRequestHeaders
        protected void Application_PreSendRequestHeaders()
        {
            HttpResponse response = HttpContext.Current.Response;
            response.Headers.Remove("X-AspNet-Version");
            response.Headers.Remove("Server");
            //if (response.Cookies.Count > 0)
            //{
            //    //if (HttpContext.Current.Request.RequestContext.RouteData.DataTokens["area"] != null)
            //    //{
            //    //    return;
            //    //}
            //    //foreach (string s in Response.Cookies.AllKeys)
            //    //{
            //    //    if (s == System.Web.Security.FormsAuthentication.FormsCookieName || s.ToLower() == "asp.net_sessionid")
            //    //    {
            //    //        Response.Cookies[s].Secure = true;
            //    //    }
            //    //}
            //}
        }
        #endregion

        #region Application_BeginRequest
        protected void Application_BeginRequest()
        {
            if (!Context.Request.IsSecureConnection)
            {
                //if (Request.ServerVariables["SERVER_PORT"].Contains("80") || Request.ServerVariables["SERVER_PORT"].Contains("443"))
                //{
                //    Response.Redirect(Context.Request.Url.ToString().Trim().Replace("http://", "https://"));
                //}
            }
        }
        #endregion

        #region Application_Error
        protected void Application_Error(object sender, EventArgs e)
        {
            var httpContext = ((MvcApplication)sender).Context;
            var area = "";
            if (httpContext.Request.RequestContext.RouteData.DataTokens["area"] != null)
            {
                area = httpContext.Request.RequestContext.RouteData.DataTokens["area"].ToString();
            }
            var currentController = " ";
            var currentAction = " ";
            var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));
            if (currentRouteData != null)
            {
                if (currentRouteData.Values["controller"] != null && !String.IsNullOrEmpty(currentRouteData.Values["controller"].ToString()))
                {
                    currentController = currentRouteData.Values["controller"].ToString();
                }

                if (currentRouteData.Values["action"] != null && !String.IsNullOrEmpty(currentRouteData.Values["action"].ToString()))
                {
                    currentAction = currentRouteData.Values["action"].ToString();
                }
            }
            var ex = Server.GetLastError();
            //var controller = new ErrorController();
            var routeData = new RouteData();
            var action = "Error";
            var tologin = false;
            if (ex is HttpException)
            {
                var httpEx = ex as HttpException;
                if (area == "")
                {
                    switch (httpEx.GetHttpCode())
                    {
                        case 404:
                            action = "FrontPageNotFound";
                            break;
                        default:
                            action = "FrontPageNotFound";
                            break;
                    }
                }
                else
                {
                    switch (httpEx.GetHttpCode())
                    {
                        case 404:
                            action = "PageNotFound";
                            break;
                        default:
                            action = "Error";
                            break;
                    }
                }
            }
            else if (ex is InvalidOperationException)
            {
                if (currentController == "Account" && currentAction == "Login")
                {
                    tologin = true;
                }
            }
            else {
                action = "FrontPageNotFound";
            }
            if (tologin == false)
            {
                httpContext.ClearError();
                httpContext.Response.Clear();
                httpContext.Response.StatusCode = ex is HttpException ? ((HttpException)ex).GetHttpCode() : 500;
                httpContext.Response.TrySkipIisCustomErrors = true;
                routeData.Values["controller"] = "Error";
                routeData.Values["action"] = action;
                routeData.Values["exception"] = new HandleErrorInfo(ex, currentController, currentAction);
                IController errormanagerController = new Controllers.ErrorController();
                HttpContextWrapper wrapper = new HttpContextWrapper(httpContext);
                var rc = new RequestContext(wrapper, routeData);
                errormanagerController.Execute(rc);
            }
            else
            {
                UrlHelper help = new UrlHelper(HttpContext.Current.Request.RequestContext);
                HttpContext.Current.ClearError();
                Response.Redirect(help.RouteUrl(new { controller = "Account", action = "Login", area = "webadmin" }), false);
                //Response.Redirect("http://yahoo.com.tw", false);
                return;
               
               
            }

            //String Errormsg = String.Empty;
            //Exception unhandledException = Server.GetLastError();
            //Response.Clear();
            //HttpException httpException = unhandledException as HttpException;
            //Errormsg = "發生列外網頁:{0}錯誤訊息:{1}堆疊內容:{2}";
            //if (httpException != null)
            //{
            //    RouteData routeData = new RouteData();
            //    routeData.Values.Add("controller", "Error");
            //    Errormsg = String.Format(Errormsg, Request.Path + Environment.NewLine,
            //        unhandledException.GetBaseException().Message + Environment.NewLine,
            //        unhandledException.StackTrace + Environment.NewLine);
            //    UrlHelper help = new UrlHelper(HttpContext.Current.Request.RequestContext);
            //    //string Host = System.Web.Configuration.WebConfigurationManager.AppSettings["Host"];
            //    switch (httpException.GetHttpCode())
            //    {
            //        case 404:

            //            Response.Redirect(help.RouteUrl(new { controller = "Home", action = "PageError", area = "" }));
            //            break;
            //        case 500:
            //            Response.Redirect(help.RouteUrl(new { controller = "Home", action = "PageError", area = "" }));
            //            break;
            //        default:
            //            Response.Redirect(help.RouteUrl(new { controller = "Home", action = "PageError", area = "" }));
            //            break;
            //    }
            //    Server.ClearError();
            //}
        } 
        #endregion
    }
}
