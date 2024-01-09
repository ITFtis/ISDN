using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;

namespace WebSiteProject.Controllers
{
    public class HomeController : AppController
    {
        MasterPageManager _IMasterPageManager;
        ILangManager _ILangManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        IMenuManager _IMenuManager;
        IModelLinkManager _IModelLinkManager;
        ADRightDownManager _ADRightDownManager;
        public HomeController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILangManager = serviceinstance.LangManager;
            _ILoginManager = serviceinstance.LoginManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelLinkManager = serviceinstance.ModelLinkManager;
            _ADRightDownManager =new ADRightDownManager(new SQLRepository<ADRightDown>(connectionstr));
        }
        // GET: WebSite/Home
        public ActionResult Index(string langid)
        {
            //Session.Abandon();
            var tempsession = Session["LoginDate"];
            if (Session["LangID"] == null)
            {
                var DefaultLang = System.Web.Configuration.WebConfigurationManager.AppSettings["DefaultLang"];
                _ILangManager = serviceinstance.LangManager;
                var alllang = _ILangManager.GetAll();
                var _langid = 1;
                if (alllang != null)
                {
                    if (alllang.Any(v => v.Lang_Name == DefaultLang))
                    {
                        _langid = alllang.Where(v => v.Lang_Name == DefaultLang).First().ID.Value;
                    }
                }
                if (string.IsNullOrEmpty(langid))
                {
                    langid = _langid.ToString();
                }
                Session["LangID"] = langid;
                Session.Timeout = 600;
            }
            else {
                if (string.IsNullOrEmpty(langid) == false)
                {
                    Session["LangID"] = langid;
                }
                else {
                    langid = Session["LangID"].ToString();
                }
            }
            HomeViewModel viewmodel = new HomeViewModel();
            //讀取logo圖片
            _IMasterPageManager.SetModel<HomeViewModel>(ref viewmodel,"P", langid, "");
            viewmodel.SEOScript = _IMasterPageManager.GetSEOData("", "", langid);
            viewmodel.ADMain = _IMasterPageManager.GetADMain("P", langid);
            viewmodel.ADMobile = _IMasterPageManager.GetADMain("M", langid);
            viewmodel.PageSeminarModel = _ISiteLayoutManager.GetPageSeminarModel();
            if (viewmodel.PageSeminarModel.BGStyle != 4) {
                viewmodel.PageSeminarModel.StyleUploadFileUrl = VirtualPathUtility.ToAbsolute("~/img/meeting_bg_0"+ viewmodel.PageSeminarModel.BGStyle+".jpg");
            }
            viewmodel.PageLayoutModel1 = _IMasterPageManager.GetNewSiteLayout("News");
            viewmodel.PageLayoutModel2 = _IMasterPageManager.GetNewSiteLayout("Active");
            viewmodel.PageLayoutModel3 = _IMasterPageManager.GetNewSiteLayout("Journal");
            viewmodel.BannerImage = "";
            viewmodel.ActiveLinkItems = _IModelLinkManager.PagingItem("Y", new SearchModelBase()
            {
                LangId = this.LangID,
                Limit = -1,
                Sort = "Sort",
                 Key="Active",
                 Offset=0
            }).rows;
            viewmodel.NewsLinkItems = _IModelLinkManager.PagingItem("Y", new SearchModelBase()
            {
                LangId = this.LangID,
                Limit = 8,
                Sort = "Sort",
                Key = "News",
                Offset=0
            }).rows;
            return View(viewmodel);
        }
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult ADRigthDownDetail(string email)
        {
            var langid = _IMasterPageManager.CheckLangID("");
            var model = _IMasterPageManager.GetModel(Device, langid,"");
            var imagestrArr = _ILoginManager.GetCaptchImage();
            ViewBag.catchstr = imagestrArr[0];
            ViewBag.langid = langid;
            ViewBag.stype = Device;
            return View(model);
        }

        [HttpGet]
        public ActionResult noJavascript()
        {
            _ISiteLayoutManager.SetNoJS(1);
            return Json(Url.Content("~/img/40x40.png"),JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoJsTemplate()
        {
            return PartialView();
        }
        public ActionResult IndexCh()
        {
            return PartialView();
        }
        public ActionResult IndexEn()
        {
            return PartialView();
        }
        public ActionResult LoginForm()
        {
            return PartialView();
        }
        public ActionResult ChangeLang(string lang)
        {
            var langlist = _ILangManager.GetAll();
            IList<Lang> uselang;
            if (lang != null)
            {
                uselang = langlist.Where(v => v.ID == int.Parse(lang)).ToList();
            }
            else
            {
                var nowlang = this.LangID;
                uselang = langlist.Where(v => v.ID != int.Parse(nowlang)).ToList();
                if (uselang.Count() > 0)
                {
                    lang = uselang.First().ID.ToString();
                }
                else
                {
                    lang = this.LangID;
                }
            }

            //這個會將整個網站切換來源，2023-02-14 關閉

            //if (uselang.Count() > 0)
            //{
            //    var dtype = uselang.First().Domain_Type;
            //    if (dtype == "1")
            //    {
            //        Session["LangID"] = lang;
            //    }
            //    else if (dtype == "2")
            //    {
            //        Session["LangID"] = uselang.First().Link_Lang_ID;
            //    }
            //    else if (dtype == "3")
            //    {
            //        return Json(uselang.First().Link_Href);
            //    }
            //    Session.Timeout = 600;
            //}
            return RedirectToAction("Index","Home");
        }
        public ActionResult SetLang(string lang)
        {
            var langlist = _ILangManager.GetAll();
            IList<Lang> uselang;
            if (lang != null)
            {
                 uselang = langlist.Where(v => v.ID == int.Parse(lang)).ToList();
            }
            else {
                var nowlang = this.LangID;
                uselang = langlist.Where(v => v.ID != int.Parse(nowlang)).ToList();
                if (uselang.Count() > 0) {
                    lang = uselang.First().ID.ToString();
                }
                else {
                    lang = this.LangID;
                }
            }
            if (uselang.Count() > 0)
            {
                var dtype = uselang.First().Domain_Type;
                if (dtype == "1")
                {
                    Session["LangID"] = lang;
                }
                else if (dtype == "2")
                {
                    Session["LangID"] = uselang.First().Link_Lang_ID;
                }
                else if (dtype == "3")
                {
                    return Json(uselang.First().Link_Href);
                }
                Session.Timeout = 600;
            }

            return Json("");
        }

        protected IAuthenticationManager Authentication
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        [AllowAnonymous]
        #region CaptchRefresh
        public ActionResult CaptchRefresh()
        {
            var imagestrArr = _ILoginManager.GetCaptchImage();
            Session["Captch"] = imagestrArr[0];
            return Json(new string[] { imagestrArr[0], imagestrArr[1] });
        }
        #endregion

        public ActionResult CheckDownload(string mid)
        {       
            return Json("");
        }

        #region FileDownLoad
        public ActionResult FileDownLoad(string mid)
        {
            var model = _IMenuManager.GetModel(mid);
            if (string.IsNullOrEmpty(model.LinkUploadFilePath))
            {
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }
            else {
                if (System.IO.File.Exists(model.LinkUploadFilePath) == false) {
                    return Redirect(Request.UrlReferrer.AbsoluteUri);
                }
            }
            string filepath = model.LinkUploadFilePath;
            string oldfilename = model.LinkUploadFileName;
            if (filepath != "")
            {
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                TempData["model"] = model;
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }

        }
        #endregion

        #region PagingItem
        public ActionResult PagingADRightDownItem(ADSearchModel model)
        {
            model.Sort = "Sort";
            var data = _ADRightDownManager.PagingForWebSite( model);
            return Json(data);
        }
        #endregion

        #region PagingItem
        public ActionResult GetAuthError()
        {
            if (Session["ShowMessage"] != null)
            {
                var message = Session["ShowMessage"];
                Session.Remove("ShowMessage");
                return Json(message);
            }
            return Json("");
        }
        #endregion

        #region SetFontSize
        public ActionResult SetFontSize(string size)
        {
            Session["fontsize"] = size;
            return Json("");
        }
        #endregion

        #region ForwardResult
        public ActionResult ForwardResult()
        {
            var title = Request.QueryString["title"];
            if (title != null) {
                ViewBag.Title= HttpUtility.UrlDecode(title);
            }
            var message = Request.QueryString["message"];
            if (message != null)
            {
                ViewBag.Message = HttpUtility.UrlDecode(message);
            }
            var sitemodel = _ISiteLayoutManager.GetSiteLayout(Device, "1");
            ViewBag.FooterString = sitemodel.FowardHtmlContent;
            ViewBag.LogoUrl = sitemodel.FowardImageUrl;
            return View();
        } 
        #endregion

    }
}