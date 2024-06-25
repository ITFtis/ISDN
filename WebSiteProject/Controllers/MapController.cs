using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;
using System.ServiceModel.Syndication;
using Utilities;
using System.Configuration;
using static WebSiteProject.FilterConfig;

namespace WebSiteProject.Controllers
{
    public class MapController : AppController
    {
        MasterPageManager _IMasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        public MapController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
        }
        public ActionResult Index(string itemid, string mid)
        {
            var langid = _IMasterPageManager.CheckLangID(mid);
            var model = new MapFrontIndexModel();
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            _IMasterPageManager.SetModel<MapFrontIndexModel>(ref model, Device, LangID, "");
            ViewBag.langid = langid;
            if (string.IsNullOrEmpty(itemid) == false)
            {
                var sitemodel = _IModelWebsiteMapManager.GetSEO(itemid);
                if (sitemodel != null)
                {
                    ViewBag.mapdesc = sitemodel.Description;
                }

            }

            //var allmenu = _IMenuManager.GetMenu("", "").Where(v => v.Status == true && v.ModelID != 6);
            var allmenu = _IMenuManager.GetMenu("", "").Where(v => v.Status == true);
            var mtype = 2;
            ViewBag.Device = Device;
            if (Device == "M") { mtype = 5; }
            var topmenu = allmenu.Where(v => v.LangID == int.Parse(LangID) && v.MenuType == mtype).OrderBy(v => v.MenuLevel).ThenBy(v => v.Sort).ToList();
            UrlHelper helper = new UrlHelper(HttpContext.Request.RequestContext);
            SQLRepository<MenuUrl> _menuurlsqlrepository = new SQLRepository<MenuUrl>(connectionstr);
            var menuurl = _menuurlsqlrepository.GetAll().ToDictionary(v => v.MenuName, v => v.MenuPath);
            foreach (var m in topmenu)
            {
                m.MenuUrl = m.ModelID==6?"#": MasterPageManager.GetMapPageUrl(menuurl, m, helper);
                //m.MenuName = m.MenuName.TrimgHtmlTag();
            }
            mtype = 1;
            if (Device == "M") { mtype = 4; }
            var mainmenu = allmenu.Where(v => v.LangID == int.Parse(LangID) && v.MenuType == mtype).OrderBy(v => v.MenuLevel).ThenBy(v => v.Sort).ToList();
            foreach (var m in mainmenu)
            {
                m.MenuUrl = m.ModelID == 6 ? "#" : MasterPageManager.GetMapPageUrl(menuurl, m, helper);
                m.MenuName = m.MenuName.TrimgHtmlTag();
            }
            var downmenu = allmenu.Where(v => v.LangID == int.Parse(LangID) && v.MenuType == 3).OrderBy(v => v.MenuLevel).ThenBy(v => v.Sort).ToList();
            foreach (var m in downmenu)
            {
                m.MenuUrl = m.ModelID == 6 ? "#" : MasterPageManager.GetMapPageUrl(menuurl, m, helper);
                m.MenuName = m.MenuName.TrimgHtmlTag();
            }
            model.UpMenulist = topmenu;
            model.MainMenulist = mainmenu;
            model.DownMenulist = downmenu;
           
            if (menu!=null)
            {
                var mapmodel = _IModelWebsiteMapManager.Where(new ModelWebsiteMapMain() { ID = int.Parse(itemid) });
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.DisplayName.IsNullorEmpty()?  menu.MenuName: menu.DisplayName;
                if (mapmodel.Count() > 0)
                {
                    var ss = mapmodel.First().ID.ToString();
                    model.EditInfo = _IModelWebsiteMapManager.GetModelByID(mapmodel.First().ID.ToString());
                }
                else {
                    model.EditInfo = _IModelWebsiteMapManager.GetModelByID("-1");
                }
            }
            else
            {
                model.EditInfo = _IModelWebsiteMapManager.GetModelByID("-1");
                model.Title = this.LangID == "1" ? "網站導覽" : "SiteMap";
            }
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.LinkStr = _IMasterPageManager.GetFrontLinkString(itemid, mid, model.Title, sitemenuid);
            model.SEOScript = _IMasterPageManager.GetSEOData("", "", LangID, model.Title);
            return View(model);
        }
    }
}