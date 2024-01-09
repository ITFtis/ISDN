using NLog;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using Utilities;
using Services;
using Services.Interface;

namespace WebSiteProject.Controllers
{
    public class AppController :Controller
    {
        public static string connectionstr = Common.connectionStr;
        protected static Logger logger = NLogManagement.SystemLog;
        public bool IsAuthenticated = false;
        public string LanguageID = "";
        public string Account = "";
        public string UserName = "";
        public string UserID = "";
        public string LangID = "1";
        public string Device = "P";
        public bool IsNojavascript = false;
        public ServiceInstances serviceinstance = new ServiceInstances(new ViewModels.DBModels.SQLRepositoryInstances(connectionstr));
        ILangManager _ILangManager;
        public AppController(){
            if (System.Web.HttpContext.Current.Session["NoJacascript"] != null)
            {
                if ((int)System.Web.HttpContext.Current.Session["NoJacascript"] == 1) {
                    IsNojavascript = true;
                }
            }

            if (System.Web.HttpContext.Current.Session["LangID"] == null)
            {
                var DefaultLang = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultLang"];
                _ILangManager = serviceinstance.LangManager;
                var alllang = _ILangManager.GetAll();
                var langid = 1;
                if (alllang != null)
                {
                    if (alllang.Any(v => v.Lang_Name == DefaultLang))
                    {
                        langid = alllang.Where(v => v.Lang_Name == DefaultLang).First().ID.Value;
                    }
                }
                System.Web.HttpContext.Current.Session["LangID"] = langid;
                System.Web.HttpContext.Current.Session.Timeout = 600;
            }
            LangID = System.Web.HttpContext.Current.Session["LangID"].ToString();
        }
        protected ActionResult ResetDirectory() {
      
            if (Request.UrlReferrer != null && Request.UrlReferrer.AbsoluteUri == Request.Url.AbsoluteUri)
            { return RedirectToAction("Index", "Home"); }
            else { if (Request.UrlReferrer != null) {
                    System.Web.HttpContext.Current.Session["ReturnUrl"] = Request.Url.AbsoluteUri;
                    if (Request.UrlReferrer.AbsoluteUri.ToLower().IndexOf("epaper/review", 0, StringComparison.Ordinal) >= 0)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else {
                        return Redirect(Request.UrlReferrer.AbsoluteUri);
                    }
                } else { return RedirectToAction("Index", "Home"); } }
        }
}
}