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
    public class PatentController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelPatentManager _IPatentManager;
        ISiteLayoutManager _ISiteLayoutManager;
        public PatentController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IPatentManager = serviceinstance.ModelPatentManager; 
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["PatentIndexNoJsSearch"] != null) { Session.Remove("PatentIndexNoJsSearch"); }
            if (this.IsNojavascript) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null) {
                if (Session["messagemodelid"].ToString() == itemid) {
                    if (Session["messagepage"] != null)
                    {
                        ViewBag.nowpage = Session["messagepage"];
                    }
                    if (Session["messagegroup"] != null)
                    {
                        ViewBag.groupid = Session["messagegroup"];
                    }
                }
            }
            Session["messagepage"] = null;
            Session["messagegroup"] = null;
            Session["messagemodelid"] = null;

            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            PatentFrontIndexModel model = new PatentFrontIndexModel();
            _MasterPageManager.SetModel<PatentFrontIndexModel>(ref model, Device, LangID, mid);
            if (model.IsNoJs == 1) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            var mainmodel = _IPatentManager.Where(new ModelPatentMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
          
            if (menu!=null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            var unitmodel = _IPatentManager.GetUnitModel(itemid);
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList= _IPatentManager.GetGroupSelectList(itemid);
            model.Hasgroup= model.GroupList.Count() == 1 ? false : true;
            model.GroupList.First().Text = Common.GetLangText("全部");
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID= string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.Intro = unitmodel.IntroductionHtml;
            return View(model);
        }
        #endregion


        #region IndexNoJs
        [NoCacheAttribute]
        public ActionResult IndexNoJs(string itemid, string mid)
        {
            var page_list = Request["page_list"].IsNullorEmpty() ? "" : Request["page_list"];
            var pindex = Request["pindex"].IsNullorEmpty() ? "1" : Request["pindex"];
            var maxpage = Request["maxpage"].IsNullorEmpty() ? "999" : Request["maxpage"];
            var GroupId = "";
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (Session["PatentIndexNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["PatentIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("PatentIndexNoJsSearch");
                    }
                }
            }
            ViewBag.GroupId = GroupId;
            ViewBag.DisplayFrom = DisplayFrom;
            ViewBag.DisplayTo = DisplayTo;
            ViewBag.keyword = keyword;
            ViewBag.pagetype = Request["pagetype"].IsNullorEmpty() ? "news_list" : Request["pagetype"];

            int nowpage = 1;
            try
            {
                nowpage = Request["nowpage"] == null ? 1 : int.Parse(Request["nowpage"]);
            }
            catch (Exception ex)
            {
                nowpage = 1;
            }

            if (pindex == "1")
            {
                nowpage = 1;
            }
            else if (pindex == "-1")
            {
                nowpage -= 1;
            }
            else if (pindex == "+1")
            {
                nowpage += 1;
            }
            else
            {
                try
                {
                    nowpage = int.Parse(pindex);
                }
                catch (Exception ex)
                {
                    nowpage = 1;
                }
            }
            if (page_list != "")
            {
                try
                {
                    nowpage = int.Parse(page_list);
                }
                catch (Exception ex)
                {
                    nowpage = 1;
                }
            }
            if (nowpage <= 0) { nowpage = 1; }
            try
            {
                var maxpagecnt = int.Parse(maxpage);
                if (nowpage > maxpagecnt) { nowpage = maxpagecnt; }
            }
            catch (Exception ex)
            {
                nowpage = 1;
            }

            var page = nowpage;
            //if(pindex)
            ViewBag.nowpage = page;
            if (string.IsNullOrEmpty(itemid))
            {
                _MasterPageManager.SetNoJS(0);
                return RedirectToAction("Index", "Home");
            }

      
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            PatentFrontIndexModel model = new PatentFrontIndexModel();
            _MasterPageManager.SetModel<PatentFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IPatentManager.Where(new ModelPatentMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                _MasterPageManager.SetNoJS(0);
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            if (menu != null)
            {
                _MasterPageManager.SetNoJS(0);
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0)
                {
                    model.Title = mainmodel.First().Name;
                }
            }
            var unitmodel = _IPatentManager.GetUnitModel(itemid);
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IPatentManager.GetGroupSelectList(itemid);
            model.Hasgroup = model.GroupList.Count() == 1 ? false : true;
            model.GroupList.First().Text = Common.GetLangText("全部");
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.Intro = unitmodel.IntroductionHtml;

            var limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value;
            var offset = ((page - 1) * limit);
            var searchmodel = new PatentSearchModel()
            {
                Sort = "Sort",
                Search = "Y",
                Limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value,
                ModelID = int.Parse(model.MainID),
                MenuId = model.MenuID,
                Offset = offset,
                GroupId = GroupId == "" ? (int?)null : int.Parse(GroupId),
                DisplayFrom = DisplayFrom,
                DisplayTo = DisplayTo,
                Title = keyword
            };
            var data = _IPatentManager.PagingItemForWebSite(searchmodel.ModelID.ToString(), searchmodel, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            foreach (var _d in data.rows)
            {
                sb.Append("<tr>");
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    if (c.Name == "年度")
                    {
                        sb.Append("<td scope = 'row' class='text-center'>" + _d.Year + "</td>");
                    }
                    else if (c.Name == "標題")
                    {
                        if (searchmodel.MenuId != "-1")
                        {
                            sb.Append("<td><a href='" + Url.Action("PatentView", new { id = _d.ItemID, mid = searchmodel.MenuId, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "'>" + _d.Title + "</ a></td>");
                        }
                        else
                        {
                            sb.Append("<td><a href='" + Url.Action("PatentView", new { id = _d.ItemID, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "'>" + _d.Title + "</ a></td>");
                        }
                    }
                    else if (c.Name == "類別")
                    {
                        sb.Append("<td class='text-center'>" + _d.GroupName + "</td>");
                    }
                }
                sb.Append("</tr>");
            }
            decimal pagecnt = -1;
            if (searchmodel.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)searchmodel.Limit);
            }
            ViewBag.Html = sb.ToString();

            var endcnt = (searchmodel.Offset + searchmodel.Limit) > data.total ? data.total : (searchmodel.Offset + searchmodel.Limit);
            ViewBag.TotalCntStr = data.total + "， " + Common.GetLangText("顯示") + " : " + (searchmodel.Offset + 1) + "~" + endcnt;
            ViewBag.maxpage = 0;
            if (data.total == 0 || unitmodel.ShowCount == -1 ) { ViewBag.showpagenum = "N"; }
            else
            {
                ViewBag.maxpage = pagecnt;
            }
            _MasterPageManager.SetNoJS(0);
            return View(model);
        }
        #endregion

        #region IndexNoJsSearch
        public ActionResult IndexNoJsSearch(string itemid, string mid)
        {
            var GroupId = Request["GroupId"].IsNullorEmpty() ? "" : Request["GroupId"];
            var DisplayFrom = Request["DisplayFrom"].IsNullorEmpty() ? "" : Request["DisplayFrom"];
            var DisplayTo = Request["DisplayTo"].IsNullorEmpty() ? "" : Request["DisplayTo"];
            var keyword = Request["keyword"].IsNullorEmpty() ? "" : Request["keyword"];
            var skey = new Dictionary<string, string>();
            if (GroupId != "" || DisplayFrom != "" || DisplayTo != "" || keyword != "")
            {
                skey.Add("GroupId", GroupId);
                skey.Add("DisplayFrom", DisplayFrom);
                skey.Add("DisplayTo", DisplayTo);
                skey.Add("keyword", keyword);
                Session["PatentIndexNoJsSearch"] = skey;
            }
            else
            {
                if (Session["PatentIndexNoJsSearch"] != null) { Session.Remove("PatentIndexNoJsSearch"); }
            }
            return RedirectToAction("IndexNoJs", "Patent", new { itemid = itemid, mid = mid });
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(PatentSearchModel model)
        {
            model.Sort = "Sort";
            var data = _IPatentManager.PagingItemForWebSite(model.ModelID.ToString(), model, "");
            var unitmodel = _IPatentManager.GetUnitModel(model.ModelID.ToString());
            var ColumnSetting = unitmodel.UnitSettingColumnList;

            var sb = new System.Text.StringBuilder();
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows) {
                sb.Append("<tr>");
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    if (c.Name == "年度")
                    {
                        sb.Append("<td scope = 'row' class='text-center'>" + _d.Year + "</td>");
                    }
                    else if (c.Name == "標題")
                    {
                        if (model.MenuId != "-1")
                        {
                            sb.Append("<td><a href='" + Url.Action("PatentView", new { id = _d.ItemID, mid = model.MenuId, page = model.NowPage, groupid = model.GroupId }) + "' title='" + _d.Title + "'>"+ _d.Title +"</ a></td>");
                        }
                        else
                        {
                            sb.Append("<td><a href='" + Url.Action("PatentView", new { id = _d.ItemID, page = model.NowPage, groupid = model.GroupId }) + "' title='" + _d.Title + "'>"+ _d.Title +"</ a></td>");
                        }
                    }
                    else if (c.Name == "類別")
                    {
                        sb.Append( "<td class='text-center'>" + _d.GroupName + "</td>");
                    }
                }
                sb.Append("</tr>");
            }
            decimal pagecnt = -1;
            if (model.Limit != -1) {
                 pagecnt = Math.Ceiling((decimal)data.total / (decimal)model.Limit);
            }         
            return Json(new string[] { sb.ToString() , data .total.ToString(), pagecnt .ToString()});
        }
        #endregion

        #region PatentView
        public ActionResult PatentView(string id, string mid,string page,string groupid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            Session["messagepage"] = page==null?"1": page;
            Session["messagegroup"] = groupid == null ? "" : groupid;
            var itemmodel = _IPatentManager.GetModelItem(id);
            if (itemmodel.ItemID == 0){return RedirectToAction("Index", "Home");}
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.IsVerift == false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            PatentFrontViewModel model = new PatentFrontViewModel();
            _MasterPageManager.SetModel<PatentFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IPatentManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IPatentManager.Where(new ModelPatentMain() { ID = itemmodel.ModelID.Value });
            if (menu!=null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.MainTitle = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0){model.MainTitle = mainmodel.First().Name;}
            }
            if (itemmodel.GroupID != null)
            {
                model.GroupID = itemmodel.GroupID.ToString();
                model.GroupName = _IPatentManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            model.SiteMenuID = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            var editmodel = _IPatentManager.GetModelByID("", id);
            model.Content = editmodel.HtmlContent==null?"": editmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");
            var fbtitle = model.Content.TrimgHtmlTag().Replace("\n", "").Replace("\t", "");
            model.FBTitle = fbtitle.Count() > 80 ? fbtitle.Substring(0, 80) : fbtitle;
            UrlHelper helper = new UrlHelper(Request.RequestContext);

            if (model.Content.IndexOf("UploadImage/PatentItem") >= 0)
            {
                model.FBImage = "";
            }
            else
            {
                var ImageFileName = helper.Content("~/img/logo_fb.jpg");
                var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = ImageFileName, Query = null, };
                model.FBImage = urlBuilder.ToString();
            }
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.PublicshDate = editmodel.PublicshStr;
            model.Title = editmodel.Title;
            model.Field = editmodel.Field;
            model.Year = editmodel.Year.IsNullorEmpty()?"" : editmodel.Year;
            model.Inventor = editmodel.Inventor;
            model.Nation = editmodel.Nation;
            model.Patentno = editmodel.Patentno;
            model.PatentDate = editmodel.PatentDate;
            model.EarlyPublicDate = editmodel.EarlyPublicDate;
            model.EarlyPublicNo = editmodel.EarlyPublicNo;
            model.Deadline = editmodel.Deadline;
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            if (itemmodel.LinkUrl.IsNullorEmpty() == false){ model.LinkUrl = itemmodel.LinkUrl;}
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false){
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
  
            return View(model);
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string itemid)
        {
            var model = _IPatentManager.GetModelItem(itemid);
            if (model.Enabled == false) { return RedirectToAction("Index", "Home"); }
            var isusedate = (model.StDate == null || model.StDate <= DateTime.Now) && (model.EdDate == null || model.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            string filepath = model.UploadFilePath;
            string oldfilename = model.UploadFileName;
            var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            if (uploadfilepath.IsNullorEmpty())
            {
                uploadfilepath = Request.PhysicalApplicationPath + "\\UploadFile";
            }
            if (filepath != "")
            {
                //取得檔案名稱
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                //讀成串流
                Stream iStream = new FileStream(uploadfilepath+filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //回傳出檔案
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        #endregion

        #region Print
        public ActionResult Print(string id, string mid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodel = _IPatentManager.GetModelItem(id);
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            PatentFrontViewModel model = new PatentFrontViewModel();
            _MasterPageManager.SetModel<PatentFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IPatentManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IPatentManager.Where(new ModelPatentMain() { ID = itemmodel.ModelID.Value });
            if (menu != null)
            {
                if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.MainTitle = menu.MenuName;
            }
            else
            {
                if (mainmodel.Count() > 0) { model.MainTitle = mainmodel.First().Name; }
            }
            if (itemmodel.GroupID != null)
            {
                model.GroupID = itemmodel.GroupID.ToString();
                model.GroupName = _IPatentManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            model.SiteMenuID = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            var editmodel = _IPatentManager.GetModelByID("", id);

            model.Content = editmodel.HtmlContent == null ? "" : editmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");
            model.PublicshDate = editmodel.PublicshStr;
            model.Title = editmodel.Title;
            model.Field = editmodel.Field;
            model.Year = editmodel.Year.IsNullorEmpty() ? "" : editmodel.Year ;
            model.Inventor = editmodel.Inventor;
            model.Nation = editmodel.Nation;
            model.Patentno = editmodel.Patentno;
            model.PatentDate = editmodel.PatentDate;
            model.EarlyPublicDate = editmodel.EarlyPublicDate;
            model.EarlyPublicNo = editmodel.EarlyPublicNo;
            model.Deadline = editmodel.Deadline;
            if (itemmodel.LinkUrl.IsNullorEmpty() == false) { model.LinkUrl = itemmodel.LinkUrl; }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            return View(model);
        }
        #endregion
    }
}