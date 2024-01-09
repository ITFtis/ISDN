using NLog;
using SQLModel;
using SQLModel.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using Utilities;
using Services;

namespace WebSiteProject.Areas.webadmin.Controllers
{
    public class AppController : Controller
    {
        public static string connectionstr = Common.connectionStr;
        protected static Logger logger=   NLogManagement.SystemLog;
        public bool IsAuthenticated = false;
        public string LanguageID = "";
        public string Account = "";
        public string UserName = "";
        public string UserID = "";
        public string GroupID = "";
        public string Role = "";
        public ServiceInstances serviceinstance = new ServiceInstances(new ViewModels.DBModels.SQLRepositoryInstances(connectionstr));
        SQLRepository<AdminFunctionAuth> _functioninputsqlrepository = new SQLRepository<AdminFunctionAuth>(connectionstr);
       SQLRepository<AdminFunction> _adfunctionrepository= new SQLRepository<AdminFunction>(connectionstr);
        public AppController() {
            if (System.Web.HttpContext.Current.Request != null) {
                if (System.Web.HttpContext.Current.Request.IsAuthenticated)
                {
                    var clang = System.Web.HttpContext.Current.Request["lang"];
                    IsAuthenticated = true;
                    var user = System.Web.HttpContext.Current.Request.GetOwinContext().Authentication.User;
                    var langclaim = user.FindFirst("Language");
                    if (user.FindFirst(System.Security.Claims.ClaimTypes.Role)==null) {
                        IsAuthenticated = false;
                        return;
                    }
                    GroupID = user.FindFirst("GroupID")==null?"": user.FindFirst("GroupID").Value;
                    if (langclaim != null) { LanguageID = langclaim.Value; }
                    Account = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                    UserName = user.Identity.Name;
                    UserID = user.FindFirst("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider").Value;
                    ViewBag.AuthList = _functioninputsqlrepository.GetByWhere("GroupID=@1 and LangID=@2 and Type=0 Order by Type,ItemID", new object[] { GroupID, LanguageID }).ToList();
                    ViewBag.GroupID = GroupID;
                    Role = user.FindFirst(System.Security.Claims.ClaimTypes.Role).Value;
                }
            }
        }

        #region CheckAuth
        public void CheckAuth(MethodBase method)
        {
            if (System.Web.HttpContext.Current.Request.IsAuthenticated == false)
            {
                return;
            }
            if (Role == "admin") { return; }
            var menuindex = Request["menuindex"];
            if (menuindex != null)
            {
                if (menuindex != "-1")
                {
                    var fdata = _functioninputsqlrepository.GetByWhere("GroupID=@1 and LangID=@2 and Type=1 and ItemID=@3", new object[] { GroupID, LanguageID, menuindex });
                    if (fdata.Count() == 0) { System.Web.HttpContext.Current.Response.Redirect(Url.Action("Index", "Home")); return; } else { return; }
                }

            }
            else
            {
                if (Session["Menuindex"] != null)
                {
                    menuindex = Session["Menuindex"].ToString();
                    if (menuindex != "-1" && menuindex.IsNullorEmpty() == false)
                    {
                        var fdata = _functioninputsqlrepository.GetByWhere("GroupID=@1 and LangID=@2 and Type=1 and ItemID=@3", new object[] { GroupID, LanguageID, menuindex });
                        if (fdata.Count() == 0) { System.Web.HttpContext.Current.Response.Redirect(Url.Action("Index", "Home")); return; }
                        else
                        {
                            if (string.IsNullOrEmpty(method.Name) == false && method.Name.ToLower() == "index")
                            {
                                Session["Menuindex"] = menuindex;
                                System.Web.HttpContext.Current.Response.Redirect(Url.Action("GotoContent", "Account")); return;
                            }
                            return;
                        }
                    }
                }
            }
            var m = System.Reflection.MethodBase.GetCurrentMethod();
            AuthoridUrlAttribute Attr =
              (AuthoridUrlAttribute)System.Attribute.GetCustomAttribute(method, typeof(AuthoridUrlAttribute));
            if (Attr != null)
            {
                var urlstr = Attr.GetUrl();
                var parastr = Attr.GetParameter();
                var action = this.RouteData.Values["action"].ToString();
                var controller = this.RouteData.Values["controller"].ToString();
                if (action.ToLower() == "index" && controller.ToLower() == "home")
                {
                    return;
                }
                var functionindex = _adfunctionrepository.GetByWhere("Url=@1", new object[] { urlstr });
                var authlist = (List<SQLModel.Models.AdminFunctionAuth>)ViewBag.AuthList;
                if (authlist == null) { return; }
                if (functionindex.Count() == 0) { System.Web.HttpContext.Current.Response.Redirect(Url.Action("Index", "Home")); return; }
                else
                {
                    if (parastr.IsNullorEmpty() == false)
                    {
                        var paraarr = parastr.Split(',');
                        var pastrlist = new List<string>();
                        foreach (var pa in paraarr)
                        {
                            if (Request[pa] == null) { System.Web.HttpContext.Current.Response.Redirect(Url.Action("Index", "Home")); return; }
                            pastrlist.Add(pa + "=" + Request[pa]);
                        }

                        var parr = string.Join("&", pastrlist);
                        foreach (var a in functionindex)
                        {
                            if (parr.Contains(a.Parameter) && authlist.Any(v => v.ItemID == a.ID))
                            {
                                return;
                            }
                        }
                        System.Web.HttpContext.Current.Response.Redirect(Url.Action("Index", "Home"));
                    }
                    else
                    {
                        if (authlist.Any(v => v.ItemID == functionindex.First().ID))
                        {
                            return;
                        }
                        else
                        {
                            System.Web.HttpContext.Current.Response.Redirect(Url.Action("Index", "Home"));
                        }
                    }
                }
            }
        } 
        #endregion
    }
}