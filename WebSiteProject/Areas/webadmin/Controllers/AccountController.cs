using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Configuration;
using System.Security.Claims;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using Utilities;
using ViewModels;
using WebSiteProject.Code;

namespace WebSiteProject.Areas.webadmin.Controllers
{

    [Authorize]
    public class AccountController : AppController
    {
        ILoginManager _ILoginManager;
        ILangManager _ILangManager;
        IMenuManager _IMenuManager;
        ISiteConfigManager _config;
        public AccountController() {
            _ILoginManager = serviceinstance.LoginManager;
            _ILangManager = serviceinstance.LangManager;
            _IMenuManager = serviceinstance.MenuManager;
            _config = serviceinstance.SiteConfigManager;
        }
        protected IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var model = new LogInViewModel()
            {
                Account = "",
                Password = ""
            };
            //model = new LogInViewModel()
            //{
            //    Account = "ken",
            //    Password = "admin",
            //    Number = "1234"
            //};

            var data = _config.GetSiteConfigModel();
           ViewBag.PageTitle = data.Page_Title;
            ViewBag.LoginTitle = data.Login_Title;
            ViewBag.CompName = data.Comp_Name;
            ViewBag.Content = data.LoginDescContent;
            ViewBag.LogoFileUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/Config/" + data.LogoUploadFileName);
            if (Session["LoginError"] != null) {
                model.Message = Session["LoginError"].ToString();
            }
            //var imagestrArr = _ILoginManager.GetCaptchImage();
            //Session["Captch"] = imagestrArr[0];
            //ViewBag.image = imagestrArr[1];
            Session.Remove("LoginError");
            Session["FromAdmin"] ="Y";  
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.v2htmlkey = System.Configuration.ConfigurationManager.AppSettings["recaptcha_sitekey_v2"];
            return View(model);
        }
        
         [AllowAnonymous]
        public ActionResult toFrontEnd()
        {
            var url = ConfigurationManager.AppSettings["FrontEndWebSite"];
            return Redirect(url);
        }

        [AllowAnonymous]
        public ActionResult CaptchRefresh()
        {
            var imagestrArr = _ILoginManager.GetCaptchImage();
            Session["Captch"] = imagestrArr[0];
            return Json(imagestrArr[1]);
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            Common.SetLogout(this.UserID);
            Authentication.SignOut();
            return RedirectToAction("Login", "Account", new { area = "webadmin" });
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LogInViewModel model, string returnUrl)
        {
            Session.Remove("LoginError");
            var loginSuccess = true;
            var data = _config.GetSiteConfigModel();
            ViewBag.PageTitle = data.Page_Title;
            ViewBag.LoginTitle = data.Login_Title;
            ViewBag.CompName = data.Comp_Name;
            ViewBag.Content = data.LoginDescContent;
            ViewBag.LogoFileUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/Config/" + data.LogoUploadFileName);
            if (Request.Form["g-recaptcha-response"].IsNullorEmpty())
            {
                loginSuccess = false;
                model.Message = "尚未勾選驗證";
            }
            else
            {
                string token = Request.Form["g-recaptcha-response"];
                var isValid = Common.ValidRecaptcha(token);
                if (isValid == false)
                {
                    loginSuccess = false;
                    model.Message = "驗證失敗";
                }
            }
            if (model.Password.IsNullorEmpty())
            {
                loginSuccess = false;
                model.Message = "請確實輸入密碼";
            }
            if (model.Account.IsNullorEmpty())
            {
                loginSuccess = false;
                model.Message = "請確實輸入帳號";
            }

            var DefaultLang = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultLang"];
            var alllang = _ILangManager.GetAll();
            var langid = 1;
            if (alllang != null)
            {
                if (alllang.Any(v => v.Lang_Name == DefaultLang))
                {
                    langid = alllang.Where(v => v.Lang_Name == DefaultLang).First().ID.Value;
                }
            }
            try
            {
                ViewData.ModelState.Clear();
           
                AdminMemberModel user = null;
                if (loginSuccess)
                {
                    model.Password = Common.GetMD5(model.Password);
                    //檢查帳號密碼
                    user = _ILoginManager.ValidateUser(model.Account, model.Password);
                    if (user == null)
                    {
                        loginSuccess = false;
                        model.Message = "登入驗證錯誤,請確認帳號或是密碼";
                    }
                    else
                    {
                        if (user.Status == false)
                        {
                            loginSuccess = false;
                            model.Message = "登入驗證錯誤,該帳號已被停用";
                        }
                        else
                        {
                            loginSuccess = true;
                        }
                    }

                }

                if (loginSuccess)
                {
                    var role = "";
                    if (user.GroupName == "總管理者") { user.GroupName = "admin"; }
                    var identity = new ClaimsIdentity(
                      new[] {
                            //new Claim(ClaimTypes.Name, user.Account),
                            new Claim(ClaimTypes.Name,user.Name),
                            new Claim(ClaimTypes.Role,user.GroupName),
                            new Claim(ClaimTypes.NameIdentifier,user.Account),
                             new Claim("GroupID", user.GroupId.ToString()),
                            new Claim("Language", langid.ToString()),
                            new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",user.ID.ToString())
                      },
                      DefaultAuthenticationTypes.ApplicationCookie,
                      ClaimTypes.Name,
                      ClaimTypes.Role);

                    Authentication.SignIn(new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.SpecifyKind(DateTime.Now.AddMinutes(180), DateTimeKind.Local)
                    }, identity);
                    Common.CreateLogin(user.ID, user.Account);
                    // AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    var imagestrArr = _ILoginManager.GetCaptchImage();
                    ViewBag.image = imagestrArr[1];
                    ViewBag.ReturnUrl = returnUrl;
                    Session["Captch"] = imagestrArr[0];
                    model.Number = "";
                    Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    Response.Cache.SetNoStore();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("登入失敗:account=" + model.Account + " error:" + ex.Message);
                model.Message = "登入失敗";
            }
            Session["LoginError"] = model.Message;
            return RedirectToAction("Login", "Account", new { area = "webadmin" });

        }

        public ActionResult SetLang(string lang)
        {
            if (Request.IsAuthenticated)
            {
                IsAuthenticated = true;
                var user = System.Web.HttpContext.Current.Request.GetOwinContext().Authentication.User;
                var Identity = (ClaimsIdentity)user.Identity;
                Identity.RemoveClaim(Identity.FindFirst("Language"));
                Identity.AddClaim(new Claim("Language", lang));
                Authentication.SignOut();

                var identity = new ClaimsIdentity(
                Identity.Claims,
                DefaultAuthenticationTypes.ApplicationCookie,
                ClaimTypes.Name,
                ClaimTypes.Role);
                Authentication.SignIn(new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.SpecifyKind(DateTime.Now.AddMinutes(20), DateTimeKind.Local)
                }, identity);

            }
            return Json("");
        }

        public ActionResult GotoMenu(string id)
        {
            return Json(_IMenuManager.GetMenuUrl(id));
        }
        public ActionResult GotoContent()
        {
            string menuindex = Session["Menuindex"].ToString();
            var _IMenuManager = serviceinstance.MenuManager;
            var urlarr = _IMenuManager.GetMenuUrl(menuindex);
            if (urlarr[1] == "")
            {
                return RedirectToAction("Index","Home");
            }
            else {
                var cttext = "<form action='" + urlarr[1] + "' id='frmTemp' method='post'><input type='hidden' name='menuindex' value='" + menuindex + "' />" +
                "<input type='hidden' name='" + urlarr[2] + "' value='" + urlarr[3] + "' /></form><script>document.getElementById('frmTemp').submit();</script>";
                return Content(cttext);
            }

        }

    }
}