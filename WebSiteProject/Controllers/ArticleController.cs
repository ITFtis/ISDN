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
using System.Net.Mail;
using System.Net;
using System.ComponentModel.DataAnnotations;
using NAudio.Wave;
using NAudio.Lame;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace WebSiteProject.Controllers
{
    public class ArticleController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelArticleManager _IModelArticleManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        public ArticleController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelArticleManager = serviceinstance.ModelArticleManager;
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILoginManager = serviceinstance.LoginManager;
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["ArticleIndexNoJsSearch"] != null) { Session.Remove("ArticleIndexNoJsSearch"); }
            if (this.IsNojavascript) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null)
            {
                if (Session["messagemodelid"].ToString() == itemid)
                {
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
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ViewBag.IntroductionHtml = unitmodel.IntroductionHtml;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            ArticleFrontIndexModel model = new ArticleFrontIndexModel();
            _MasterPageManager.SetModel<ArticleFrontIndexModel>(ref model, Device, LangID, mid);
            if (model.IsNoJs == 1) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            if (menu != null)
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IModelArticleManager.GetGroupSelectList(itemid);
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
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            if (unitmodel.Summary.IsNullorEmpty())
            {
                model.tablesummary = "此為" + model.Title + Common.GetLangText("列表") + "，";
                var idx = 0;
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    idx += 1;
                    if (c.Name == "序號") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["序號"] + "，"); }
                    if (c.Name == "發佈日期") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["發佈日期"] + "，"); }
                    if (c.Name == "標題") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["標題"] + "，"); }
                    if (c.Name == "類別") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["類別"] + "，"); }
                    if (c.Name == "相關連結") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["相關連結"] + "，"); }
                    if (c.Name == "檔案下載") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["檔案下載"] + "，"); }
                }
                model.tablesummary = model.tablesummary.TrimEnd('，');
            }
            else
            {
                model.tablesummary = unitmodel.Summary;
            }
            return View(model);
        }
        #endregion

        #region GroupIndex
        [NoCacheAttribute]
        public ActionResult GroupIndex(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["ArticleIndexNoJsSearch"] != null) { Session.Remove("ArticleIndexNoJsSearch"); }
            if (this.IsNojavascript) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null)
            {
                if (Session["messagemodelid"].ToString() == itemid)
                {
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
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ViewBag.IntroductionHtml = unitmodel.IntroductionHtml;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            ArticleFrontIndexModel model = new ArticleFrontIndexModel();
            _MasterPageManager.SetModel<ArticleFrontIndexModel>(ref model, Device, LangID, mid);
            if (model.IsNoJs == 1) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            if (menu != null)
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IModelArticleManager.GetGroupSelectList(itemid);
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
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            return View(model);
        }
        #endregion

        #region IndexNoJs
        [NoCacheAttribute]
        public ActionResult IndexNoJs(string itemid, string mid)
        {
            //if (Request.HttpMethod.ToLower() == "post")
            //{
            //    System.Web.Helpers.AntiForgery.Validate();
            //}
            var page_list = Request["page_list"].IsNullorEmpty() ? "" : Request["page_list"];
            var pindex = Request["pindex"].IsNullorEmpty() ? "1" : Request["pindex"];
            var maxpage = Request["maxpage"].IsNullorEmpty() ? "999" : Request["maxpage"];
            var GroupId = "";
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (Session["ArticleIndexNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["ArticleIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("ArticleIndexNoJsSearch");
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
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ViewBag.IntroductionHtml = unitmodel.IntroductionHtml;
            ArticleFrontIndexModel model = new ArticleFrontIndexModel();
            _MasterPageManager.SetModel<ArticleFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = int.Parse(itemid) });
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IModelArticleManager.GetGroupSelectList(itemid);
            model.GroupList.First().Text = Common.GetLangText("全部");
            model.Hasgroup = model.GroupList.Count() == 1 ? false : true;
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            var idx = 0;
            if (unitmodel.Summary.IsNullorEmpty())
            {
                model.tablesummary = "此為" + model.Title + Common.GetLangText("列表") + "，";

                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    idx += 1;
                    if (c.Name == "序號") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["序號"] + "，"); }
                    if (c.Name == "發佈日期") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["發佈日期"] + "，"); }
                    if (c.Name == "標題") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["標題"] + "，"); }
                    if (c.Name == "類別") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["類別"] + "，"); }
                    if (c.Name == "相關連結") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["相關連結"] + "，"); }
                    if (c.Name == "檔案下載") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["檔案下載"] + "，"); }
                }
                model.tablesummary = model.tablesummary.TrimEnd('，');
            }
            else
            {
                model.tablesummary = unitmodel.Summary;
            }
            var limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value;
            var offset = ((page - 1) * limit);
            var searchmodel = new ArticleSearchModel()
            {
                Sort = "Sort",
                Search = "Y",
                Limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value,
                ModelID = int.Parse(model.MainID),
                MenuId = model.MenuID,
                Offset = offset,
                //GroupId = GroupId == "" ? (int?)null : int.Parse(GroupId),
                GroupId = GroupId == null ? "" : GroupId,
                DisplayFrom = DisplayFrom,
                DisplayTo = DisplayTo,
                Title = keyword
            };
            var data = _IModelArticleManager.PagingItemForWebSite(searchmodel.ModelID.ToString(), searchmodel, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            idx = offset;
            foreach (var _d in data.rows)
            {
                sb.Append("<tr>");
                idx += 1;
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    if (c.Name == "序號") { sb.Append("<td class='text-center'>" + idx + "</td>"); }
                    if (c.Name == "發佈日期") { sb.Append("<td class='text-center'>" + _d.PublicshDate + "</td>"); }

                    if (c.Name == "標題")
                    {
                        if (_d.Link_Mode == 1)
                        {

                            if (model.MenuID != "-1")
                            {
                                sb.Append("<td><a href='" + Url.Action("ArticleView", new { id = _d.ItemID, mid = model.MenuID, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) +
                                    "' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                            else
                            {
                                sb.Append("<td><a href='" + Url.Action("ArticleView", new { id = _d.ItemID, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) +
                                  "' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                        }
                        else
                        {
                            if (_d.LinkUrl.IsNullorEmpty())
                            {
                                sb.Append("<td><a href='#' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                            else
                            {
                                sb.Append("<td><a href='" + _d.LinkUrl + "' title='" + _d.Title + "(另開新視窗)' target='_blank'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                        }
                    }
                    if (c.Name == "類別") { sb.Append("<td class='text-center'>" + (_d.GroupName == null ? "" : _d.GroupName) + "</td>"); }
                    if (c.Name == "相關連結")
                    {
                        if (_d.LinkUrl.IsNullorEmpty())
                        {
                            sb.Append("<td class='text-center'></td>");
                        }
                        else
                        {
                            sb.Append("<td class='text-center'>");
                            sb.Append("<a href = '" + _d.LinkUrl + "' title='相關連結-" + _d.LinkUrlDesc + "(另開新視窗)'  target='_blank'>");
                            sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true'><i class='fa fa-square fa-stack-2x font-blue-steel'  aria-hidden='true'></i><i class='fas fa-link fa-stack-1x'  aria-hidden='true'></i>");
                            sb.Append("</span><span class='sr-only'>相關連結</span></a></td>");
                        }
                    }
                    if (c.Name == "檔案下載")
                    {
                        if (_d.UploadFileName.IsNullorEmpty())
                        {
                            sb.Append("<td class='text-center'></td>");
                        }
                        else
                        {
                            sb.Append("<td class='text-center'>");
                            sb.Append("<a href = '" + Url.Action("FileDownLoad", new { itemid = _d.ItemID }) + "' title='檔案下載-" + _d.FileUrlDesc + ".pdf(另開新視窗)'  target='_blank'>");
                            sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true'><i class='fa fa-square fa-stack-2x font-blue-steel'  aria-hidden='true'></i><i class='fas fa-download fa-stack-1x'  aria-hidden='true'></i>");
                            sb.Append("</span><span class='sr-only'>檔案下載</span></a></td>");
                        }

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
            if (data.total == 0 || unitmodel.ShowCount == -1) { ViewBag.showpagenum = "N"; }
            else
            {
                ViewBag.maxpage = pagecnt;
            }
            _MasterPageManager.SetNoJS(0);
            return View(model);
        }
        #endregion

        #region IndexNoJsSearch
        public ActionResult IndexNoJsSearch(string itemid, string mid, string groupid, string subgroupid)
        {
            //if (Request.HttpMethod.ToLower() == "post")
            //{
            //    System.Web.Helpers.AntiForgery.Validate();
            //}
            var GroupId = Request["GroupId"].IsNullorEmpty() ? "" : Request["GroupId"];
            var SubGroupId = Request["SubGroupId"].IsNullorEmpty() ? "" : Request["SubGroupId"];
            var DisplayFrom = Request["DisplayFrom"].IsNullorEmpty() ? "" : Request["DisplayFrom"];
            var DisplayTo = Request["DisplayTo"].IsNullorEmpty() ? "" : Request["DisplayTo"];
            var keyword = Request["keyword"].IsNullorEmpty() ? "" : Request["keyword"];
            var skey = new Dictionary<string, string>();
            if (GroupId != "" || DisplayFrom != "" || DisplayTo != "" || keyword != "")
            {
                skey.Add("GroupId", GroupId);
                skey.Add("SubGroupId", SubGroupId);
                skey.Add("DisplayFrom", DisplayFrom);
                skey.Add("DisplayTo", DisplayTo);
                skey.Add("keyword", keyword);
                Session["ArticleIndexNoJsSearch"] = skey;
            }
            else
            {
                if (Session["ArticleIndexNoJsSearch"] != null) { Session.Remove("ArticleIndexNoJsSearch"); }
            }
            if (groupid.IsNullorEmpty())
            {
                return RedirectToAction("IndexNoJs", "Message", new { itemid = itemid, mid = mid });
            }
            else
            {
                return RedirectToAction("GroupItemListNoJs", "Message", new { itemid = itemid, mid = mid, groupid = groupid, subgroupid = subgroupid });
            }

        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(ArticleSearchModel model)
        {
            model.Sort = "Sort";
            var data = _IModelArticleManager.PagingItemForWebSite(model.ModelID.ToString(), model, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/img/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            var unitmodel = _IModelArticleManager.GetUnitModel(model.ModelID.ToString());
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            var idx = model.Offset;
            foreach (var _d in data.rows)
            {
                sb.Append("<tr>");
                idx += 1;
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    if (c.Name == "序號") { sb.Append("<td class='text-center'>" + idx + "</td>"); }
                    if (c.Name == "發佈日期") { sb.Append("<td class='text-center'>" + _d.PublicshDate + "</td>"); }

                    if (c.Name == "標題")
                    {
                        if (_d.Link_Mode == 1)
                        {
                            if (model.MenuId != "-1")
                            {
                                sb.Append("<td><a href='" + Url.Action("ArticleView", new { id = _d.ItemID, mid = model.MenuId, page = model.NowPage, groupid = model.GroupId, subgroup = model.SubGroupId }) +
                                    "' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                            else
                            {
                                sb.Append("<td><a href='" + Url.Action("ArticleView", new { id = _d.ItemID, page = model.NowPage, groupid = model.GroupId, subgroup = model.SubGroupId }) +
                                  "' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                        }
                        else
                        {
                            if (_d.LinkUrl.IsNullorEmpty())
                            {
                                sb.Append("<td><a href='#' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                            else
                            {
                                sb.Append("<td><a href='" + _d.LinkUrl + "' title='" + _d.Title + "(另開新視窗)' target='_blank'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                            }
                        }
                    }
                    if (c.Name == "類別") { sb.Append("<td class='text-center'>" + (_d.GroupName == null ? "" : _d.GroupName) + "</td>"); }
                    if (c.Name == "相關連結")
                    {
                        if (_d.LinkUrl.IsNullorEmpty())
                        {
                            sb.Append("<td class='text-center'></td>");
                        }
                        else
                        {
                            sb.Append("<td class='text-center'>");
                            sb.Append("<a href = '" + _d.LinkUrl + "' title='相關連結-" + _d.LinkUrlDesc + "(另開新視窗)'  target='_blank'>");
                            sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true'><i class='fa fa-square fa-stack-2x font-blue-steel'  aria-hidden='true'></i><i class='fas fa-link fa-stack-1x'  aria-hidden='true'></i>");
                            sb.Append("</span><span class='sr-only'>相關連結</span></a></td>");
                        }
                    }
                    if (c.Name == "檔案下載")
                    {
                        if (_d.UploadFileName.IsNullorEmpty())
                        {
                            sb.Append("<td class='text-center'></td>");
                        }
                        else
                        {
                            sb.Append("<td class='text-center'>");
                            sb.Append("<a href = '" + Url.Action("FileDownLoad", new { itemid = _d.ItemID }) + "' title='檔案下載-" + _d.FileUrlDesc + ".pdf(另開新視窗)'  target='_blank'>");
                            sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true'><i class='fa fa-square fa-stack-2x font-blue-steel'  aria-hidden='true'></i><i class='fas fa-download fa-stack-1x'  aria-hidden='true'></i>");
                            sb.Append("</span><span class='sr-only'>檔案下載</span></a></td>");
                        }

                    }
                }
                sb.Append("</tr>");
            }
            decimal pagecnt = -1;
            if (model.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)model.Limit);
            }
            return Json(new string[] { sb.ToString(), data.total.ToString(), pagecnt.ToString() });
        }
        #endregion

        #region ArticleView
        public ActionResult ArticleView(string id, string mid, string page, string groupid, string subgroupid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.id = id;
            ViewBag.mid = mid;
            ViewBag.page = page;
            ViewBag.groupid = groupid;
            ViewBag.subgroupid = subgroupid;
            Session["messagepage"] = page == null ? "1" : page;
            Session["messagegroup"] = groupid == null ? "" : groupid;
            var itemmodel = _IModelArticleManager.GetModelItem(id);
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.IsVerift == false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            ArticleFrontViewModel model = new ArticleFrontViewModel();
            _MasterPageManager.SetModel<ArticleFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelArticleManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = itemmodel.ModelID.Value });
            if (string.IsNullOrEmpty(mid) == false)
            {
                var menu = _IMenuManager.GetModel(mid);
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
                model.GroupName = _IModelArticleManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            if (itemmodel.RelateImageFileName.IsNullorEmpty() == false)
            {
                itemmodel.RelateImageFileName = helper.Content("~/UploadImage/ArticleItem/" + itemmodel.RelateImageFileName);
            }
            else
            {
                itemmodel.RelateImageFileName = baseimg;
            }
            var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = itemmodel.RelateImageFileName, Query = null, };
            model.FBImage = urlBuilder.ToString();
            model.SiteMenuID = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            model.Content = itemmodel.HtmlContent == null ? "<div></div>" :
              itemmodel.HtmlContent
              .Replace("\n", "<br>")
              .Replace("<<br>", "<\n")
              .Replace("><br>", ">\n")
              .Replace("\n", "")
              .Replace("\t", "")

              // 解決W3C問題
              .Replace("<font", "<b")
              .Replace("/font", "b")

              // 解決開源格式問題
              .Replace(".docx", ".odt")
              .Replace(".doc", ".odt");

            // 轉換成html格式
            HtmlDocument xdoc = new HtmlDocument();
            xdoc.LoadHtml(model.Content);

            // 針對 img 去修正
            var imageNodes = xdoc.DocumentNode.Descendants("img").ToList();
            for (int index = 0; index < imageNodes.Count; index++)
            {
                var node = imageNodes[index];
                if (!node.Attributes.Where(e => e.Name == "alt").Any())
                {
                    //node.Remove(); //為了解決無障礙問題
                    //node.SetAttributeValue("alt", MessageController.getName());
                }
            }

            // 針對table的修正
            var tableNodes = xdoc.DocumentNode.Descendants("table");
            foreach (var table in tableNodes)
            {

                //判斷是否有header
                if (!table.Descendants("thead").Any())
                {
                    var body = table.Descendants("tbody").FirstOrDefault();
                    if (body != null)
                    {
                        var head = HtmlNode.CreateNode("<thead></thead>");

                        var firstRow = body.Descendants("tr").FirstOrDefault();
                        head.AppendChild(firstRow); // 複製第一row到head去
                        body.RemoveChild(firstRow); // 移除第一row

                        table.PrependChild(head);
                    }
                }

                // 替換head內的標籤
                if (table.Descendants("thead").Any())
                {
                    var head = table.Descendants("thead").FirstOrDefault();
                    var headString = head.OuterHtml.Replace("<td", "<th").Replace("</td>", "</th>");
                    table.RemoveChild(head);

                    var newHead = HtmlNode.CreateNode(headString);
                    table.PrependChild(newHead);
                }
            }
            model.Content = xdoc.DocumentNode.OuterHtml;



            var fbtitle = model.Content.TrimgHtmlTag();
            model.FBTitle = fbtitle.Count() > 80 ? fbtitle.Substring(0, 80) : fbtitle;

            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.PublicshDate = itemmodel.PublicshDate.Value.ToString("yyyy/MM/dd");
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.Title = itemmodel.Title;
            model.ArticleLink = itemmodel.ArticleLink;
            model.ArticleHasMore = itemmodel.IsShowMoreArticle.Value;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            if (model.ArticleHasMore)
            {
                var vstrarr = _IModelArticleManager.GetArticleMore(model.MainID, model.ItemID, model.MenuID);
                model.ArticleMore = vstrarr[0];
                model.ArticleMoreNoScript = vstrarr[1];
            }
            if (model.ArticleLink.IsNullorEmpty() == false)
            {
                if (model.ArticleLink.IndexOf("embed") != 0)
                {
                    var keyindex = model.ArticleLink.IndexOf("watch?v=");
                    var key = "";
                    if (keyindex == -1) { key = model.ArticleLink; }
                    else
                    {
                        key = model.ArticleLink.Substring(keyindex + 8);
                    }
                    model.ArticleLink = "https://www.youtube.com/embed/" + key;
                }
            }


            if (itemmodel.LinkUrl.IsNullorEmpty() == false) { model.LinkUrl = itemmodel.LinkUrl; }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            model.UploadFileName = itemmodel.UploadFileName == null ? "" : itemmodel.UploadFileName;
            _IModelArticleManager.UpdateClickCount(id);
            model.ExtItem = _IModelArticleManager.GetExtItemObj(itemmodel.ModelID.ToString(), itemmodel.ExtItemList);
            model.GroupNameList = _IModelArticleManager.GetNameList(itemmodel.ItemID.ToString());
            return View(model);
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string itemid)
        {
            var model = _IModelArticleManager.GetModelItem(itemid);
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
                Stream iStream = new FileStream(uploadfilepath + filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
                //回傳出檔案
                return File(iStream, "application/octet-stream", oldfilename);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
        #endregion

        #region Forward
        public ActionResult Forward(string itemid, string mid, string menutype, string sitemenuid, string finfo)
        {
            if (finfo.IsNullorEmpty() == false)
            {
                var focusid = "";
                var info = JsonConvert.DeserializeObject<Dictionary<string, string>>(finfo);
                if (info.ContainsKey("result")) { ViewBag.result = info["result"]; }
                if (info.ContainsKey("Sender")) { ViewBag.SenderError = "Y"; if (focusid == "") { focusid = "Sender"; } }
                if (info.ContainsKey("SenderEMail")) { ViewBag.SenderEMailError = "Y"; if (focusid == "") { focusid = "SenderEMail"; } }
                if (info.ContainsKey("SenderEMailFormat")) { ViewBag.SenderEMailFormatError = "Y"; if (focusid == "") { focusid = "SenderEMail"; } }
                if (info.ContainsKey("ForwardEMail")) { ViewBag.ForwardEMailError = "Y"; if (focusid == "") { focusid = "ForwardEMail"; } }
                if (info.ContainsKey("ForwardEMailFormat")) { ViewBag.ForwardEMailFormatError = "Y"; if (focusid == "") { focusid = "ForwardEMail"; } }

                if (info.ContainsKey("CaptchError")) { ViewBag.CaptchError = "Y"; if (focusid == "") { focusid = "img_captch"; } }
                if (info.ContainsKey("CaptchInputError")) { ViewBag.CaptchInputError = "Y"; if (focusid == "") { focusid = "img_captch"; } }
                if (info.ContainsKey("errorinfo")) { ViewBag.errorinfo = info["errorinfo"]; }
                if (info.ContainsKey("sendstr"))
                {
                    var sendstr = info["sendstr"].Split('^');
                    if (sendstr.Length > 3)
                    {
                        ViewBag.Sender = sendstr[0];
                        ViewBag.SenderEMail = sendstr[1];
                        ViewBag.ForwardEMail = sendstr[2];
                        ViewBag.ForwardMessage = sendstr[3];
                    }

                }
                ViewBag.FocusID = focusid;
            }
            Session.Remove("ForwardInfo");
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid);
            var sitemodel = _ISiteLayoutManager.GetSiteLayout(Device, "1");
            ViewBag.FooterString = sitemodel.FowardHtmlContent;
            ViewBag.LogoUrl = sitemodel.FowardImageUrl;
            ViewBag.itemid = itemid;
            ViewBag.mid = mid;
            ViewBag.menutype = menutype;
            ViewBag.sitemenuid = sitemenuid;
            var itemmodel = _IModelArticleManager.GetModelItem(itemid);
            if (itemmodel == null) { return RedirectToAction("Index", "Home"); }
            if (itemmodel != null)
            {
                ViewBag.Title = itemmodel.Title;
            }
            var hostUrl = string.Format("{0}://{1}",
              Request.Url.Scheme,
              Request.Url.Authority);
            if (sitemenuid != "-1")
            {
                ViewBag.Url = hostUrl + Url.Action("ArticleView", "Article", new { id = itemid, mid = mid, sitemenuid = sitemenuid, menutype = menutype });
            }
            else if (string.IsNullOrEmpty(mid) == false)
            {
                ViewBag.Url = hostUrl + Url.Action("ArticleView", "Article", new { id = itemid, mid = mid });
            }
            else
            {
                ViewBag.Url = hostUrl + Url.Action("ArticleView", "Article", new { id = itemid });
            }
            var imagestrArr = _ILoginManager.GetCaptchImage();
            ViewBag.audio = Url.Action("GetAudio");
            ViewBag.captch = imagestrArr[0];
            ViewBag.captchimg = imagestrArr[1];
            return View();
        }
        #endregion

        #region Print
        public ActionResult Print(string id, string mid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            var itemmodel = _IModelArticleManager.GetModelItem(id);
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
            ArticleFrontViewModel model = new ArticleFrontViewModel();
            _MasterPageManager.SetModel<ArticleFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelArticleManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = itemmodel.ModelID.Value });
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
                model.GroupName = _IModelArticleManager.GetGroupName(itemmodel.GroupID.ToString());
            }
            model.SiteMenuID = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(id, mid, mainmodel.First().Name, model.SiteMenuID);
            model.Content = itemmodel.HtmlContent == null ? "" : itemmodel.HtmlContent.Replace("\n", "<br>").Replace("<<br>", "<\n").Replace("><br>", ">\n");
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.PublicshDate = itemmodel.PublicshDate.Value.ToString("yyyy.MM.dd");
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            model.Title = itemmodel.Title;
            if (itemmodel.LinkUrl.IsNullorEmpty() == false) { model.LinkUrl = itemmodel.LinkUrl; }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.UploadFileName = itemmodel.UploadFileName == null ? "" : itemmodel.UploadFileName;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            model.ExtItem = _IModelArticleManager.GetExtItemObj(itemmodel.ModelID.ToString(), itemmodel.ExtItemList);
            model.GroupNameList = _IModelArticleManager.GetNameList(itemmodel.ItemID.ToString());
            return View(model);
        }
        #endregion

        #region SendMail
        public ActionResult SendMail(string Sender, string SenderEMail, string ForwardEMail, string ForwardMessage, string Url, string Title,
            string itemid, string mid, string menutype, string sitemenuid, string captch, string img_captch)
        {
            var cancel = Request.Form["cancel"];
            var info = new Dictionary<string, string>();
            if (cancel == null)
            {

                try
                {
                    var echeck = new EmailAddressAttribute();
                    var sendstr = new List<string>();
                    sendstr.Add(Sender == null ? "" : Sender);
                    sendstr.Add(SenderEMail == null ? "" : SenderEMail);
                    sendstr.Add(ForwardEMail == null ? "" : ForwardEMail);
                    sendstr.Add(ForwardMessage == null ? "" : ForwardMessage);
                    if (Sender.IsNullorEmpty())
                    {
                        info.Add("Sender", "");
                    }
                    if (SenderEMail.IsNullorEmpty())
                    {
                        info.Add("SenderEMail", "");
                    }
                    else
                    {
                        if (echeck.IsValid(SenderEMail) == false)
                        {
                            info.Add("SenderEMailFormat", "");
                        }
                    }
                    if (ForwardEMail.IsNullorEmpty())
                    {
                        info.Add("ForwardEMail", "");
                    }
                    else
                    {
                        var fsplit = ForwardEMail.Split(',');
                        foreach (var v in fsplit)
                        {
                            if (echeck.IsValid(v) == false)
                            {
                                if (v == "") { continue; }
                                info.Add("ForwardEMailFormat", "");
                            }
                        }
                    }
                    if (img_captch.IsNullorEmpty())
                    {
                        info.Add("CaptchError", "");
                    }
                    else
                    {
                        if (img_captch != captch)
                        {
                            info.Add("CaptchInputError", "");
                        }
                    }
                    if (info.Count() == 0)
                    {
                        var host = System.Web.Configuration.WebConfigurationManager.AppSettings["smtphost"];
                        var mailfrom = System.Web.Configuration.WebConfigurationManager.AppSettings["mailfrom"];
                        var NoticeSenderEMail = mailfrom;
                        var NoticeSubject = Title;
                        var slist = ForwardEMail.Split(';');
                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(SenderEMail, Sender);
                        foreach (var sender in slist)
                        {
                            message.To.Add(new MailAddress(sender));
                        }
                        message.SubjectEncoding = System.Text.Encoding.UTF8;
                        message.Subject = NoticeSubject;
                        message.BodyEncoding = System.Text.Encoding.UTF8;
                        string body = Sender + Common.GetLangText("寄了一則訊息給你喔") + "<br/> " + Common.GetLangText("給您的訊息") + ":" + ForwardMessage +
                            "<br/>" + Url;
                        message.Body = body;
                        message.IsBodyHtml = true;
                        message.Priority = MailPriority.High;
                        var ur = System.Web.Configuration.WebConfigurationManager.AppSettings["mailuser"];
                        var pw = System.Web.Configuration.WebConfigurationManager.AppSettings["mailpassword"];
                        var port = System.Web.Configuration.WebConfigurationManager.AppSettings["mailport"];
                        var rmessage = "";
                        try
                        {
                            if (string.IsNullOrEmpty(pw) == false)
                            {
                                SmtpClient client = new SmtpClient(host, int.Parse(port));
                                client.EnableSsl = true;
                                client.Credentials = new NetworkCredential(ur, pw);
                                client.Send(message);
                            }
                            else
                            {
                                SmtpClient client2 = new SmtpClient(host);
                                client2.Send(message);
                            }
                            rmessage = "轉寄好友寄送成功";
                        }
                        catch (Exception ex)
                        {
                            rmessage = "轉寄好友寄送失敗:" + ex.Message;
                        }
                        return RedirectToAction("ForwardResult", "Home", new { title = Title, message = rmessage });
                    }
                    else
                    {
                        info.Add("result", "error"); info.Add("sendstr", string.Join("^", sendstr));
                    }
                }
                catch (Exception ex)
                {
                    info.Add("result", "exception");
                    info.Add("errorinfo", "寄信失敗:" + ex.Message);
                }
            }
            return RedirectToAction("Forward", new { itemid = itemid, mid = mid, menutype = menutype, sitemenuid = sitemenuid, finfo = JsonConvert.SerializeObject(info) });
        }
        #endregion
        #region GetAudio
        public async Task<ActionResult> GetAudio(string text)
        {
            Task<FileContentResult> task = Task.Run(() =>
            {
                using (SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer())
                {
                    MemoryStream stream = new MemoryStream();

                    speechSynthesizer.SetOutputToWaveStream(stream);
                    var textarr = text.ToArray();
                    foreach (var t in textarr)
                    {
                        speechSynthesizer.Speak(t.ToString());
                    }
                    var bytes = stream.GetBuffer();
                    var mp3bytes = ConvertWavStreamToMp3File(ref stream, Server.MapPath("/UploadImage/fileName.mp3"));
                    return File(mp3bytes, "audio/mpeg");
                }
            });
            return await task;
        }

        #endregion
        #region ConvertWavStreamToMp3File
        private byte[] ConvertWavStreamToMp3File(ref MemoryStream ms, string savetofilename)
        {
            CheckAddBinPath();
            ms.Seek(0, SeekOrigin.Begin);
            MemoryStream msmp3 = new MemoryStream();
            using (var retMs = new MemoryStream())
            using (var rdr = new WaveFileReader(ms))
            using (var wtr = new LameMP3FileWriter(msmp3, rdr.WaveFormat, LAMEPreset.VBR_90))
            {
                rdr.CopyTo(wtr);
            }
            return msmp3.ToArray();
        }
        public void CheckAddBinPath()
        {
            var binPath = Path.Combine(new string[] { AppDomain.CurrentDomain.BaseDirectory, "bin" });
            var path = Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!path.Split(Path.PathSeparator).Contains(binPath, StringComparer.CurrentCultureIgnoreCase))
            {
                path = string.Join(Path.PathSeparator.ToString(), new string[] { path, binPath });
                Environment.SetEnvironmentVariable("PATH", path);
            }
        }
        #endregion

        #region GroupList
        [NoCacheAttribute]
        public ActionResult GroupList(string itemid, string mid)
        {
            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["ArticleIndexNoJsSearch"] != null) { Session.Remove("ArticleIndexNoJsSearch"); }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null)
            {
                if (Session["messagemodelid"].ToString() == itemid)
                {
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
            ArticleFrontGroupListModel model = new ArticleFrontGroupListModel();
            _MasterPageManager.SetModel<ArticleFrontGroupListModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.SiteMenuID = sitemenuid;
            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.MenuType = menutype;
            if (menu != null)
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.GroupArticle = _IModelArticleManager.GetAllGroupList(itemid).Where(v => v.Enabled == true).ToList();
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            return View(model);
        }
        #endregion

        #region SubGroupList
        [NoCacheAttribute]
        public ActionResult SubGroupList(string groupid, string mid)
        {
            if (string.IsNullOrEmpty(groupid))
            {
                return RedirectToAction("Index", "Home");
            }
            if (Session["ArticleIndexNoJsSearch"] != null) { Session.Remove("ArticleIndexNoJsSearch"); }

            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            var GroupData = _IModelArticleManager.GetGroupByID(groupid);
            if (GroupData.HasSubGroup == false)
            {
                return RedirectToAction("GroupItemList", new { itemid = GroupData.Main_ID, mid = mid, groupid = groupid });
            }
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            ArticleFrontGroupListModel model = new ArticleFrontGroupListModel();
            _MasterPageManager.SetModel<ArticleFrontGroupListModel>(ref model, Device, LangID, mid);
            var groupdata = _IModelArticleManager.GetGroupByID(groupid);
            var itemid = groupdata.Main_ID;
            if (itemid == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = int.Parse(itemid.ToString()) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.SiteMenuID = sitemenuid;
            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.MenuType = menutype;
            if (menu != null)
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid.ToString(), mid, mainmodel.First().Name, sitemenuid);
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid.ToString());
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.MainID = itemid.ToString();
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);

            model.SubGroupArticle = _IModelArticleManager.GetSubGroupList(groupid);
            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;

            return View(model);
        }
        #endregion

        #region GroupItemList
        [NoCacheAttribute]
        public ActionResult GroupItemList(string itemid, string mid, string groupid, string subgroupid)
        {
            if (string.IsNullOrEmpty(groupid))
            {
                return RedirectToAction("Index", "Home");
            }
            var groupdata = _IModelArticleManager.GetGroupByID(groupid);
            if (itemid.IsNullorEmpty())
            {
                itemid = groupdata.Main_ID.ToString();
            }

            if (Session["ArticleIndexNoJsSearch"] != null) { Session.Remove("ArticleIndexNoJsSearch"); }
            if (this.IsNojavascript) { return RedirectToAction("GroupItemListNoJs", new { itemid = itemid, mid = mid, groupid = groupid, subgroupid = subgroupid }); }
            ViewBag.nowpage = "1";
            ViewBag.groupid = "";
            if (Session["messagemodelid"] != null)
            {
                if (Session["messagemodelid"].ToString() == itemid)
                {
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
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ViewBag.IntroductionHtml = unitmodel.IntroductionHtml;
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            ArticleFrontIndexModel model = new ArticleFrontIndexModel();
            _MasterPageManager.SetModel<ArticleFrontIndexModel>(ref model, Device, LangID, mid);
            if (model.IsNoJs == 1) { return RedirectToAction("GroupItemListNoJs", new { itemid = itemid, mid = mid, groupid = groupid, subgroupid = subgroupid }); }
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            if (menu != null)
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
            model.GroupID = groupid == null ? "" : groupid;
            model.SubGroupID = subgroupid == null ? "" : subgroupid;
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            if (model.SubGroupID != "")
            {
                var subgroup = _IModelArticleManager.GetSubGroupByID(model.SubGroupID);
                model.Title = subgroup.First().Group_Name;
                model.LinkStr = model.LinkStr.Replace("class='active'", "") +
                    "<i class='fa fa-angle-right' aria-hidden='true' aria-label='前往其他頁面' >" +
                    "</i><span class='active' aria-label='前往其他頁面'>" + model.Title + "</span>";
            }

            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IModelArticleManager.GetGroupSelectList(itemid);
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
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            if (unitmodel.Summary.IsNullorEmpty())
            {
                model.tablesummary = "此為" + model.Title + Common.GetLangText("列表") + "，";
                var idx = 0;
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    idx += 1;
                    if (c.Name == "序號") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["序號"] + "，"); }
                    if (c.Name == "發佈日期") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["發佈日期"] + "，"); }
                    if (c.Name == "標題") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["標題"] + "，"); }
                    if (c.Name == "類別") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["類別"] + "，"); }
                    if (c.Name == "相關連結") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["相關連結"] + "，"); }
                    if (c.Name == "檔案下載") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["檔案下載"] + "，"); }
                }
                model.tablesummary = model.tablesummary.TrimEnd('，');
            }
            else
            {
                model.tablesummary = unitmodel.Summary;
            }
            return View(model);
        }
        #endregion

        #region GroupItemListNoJs
        [NoCacheAttribute]
        public ActionResult GroupItemListNoJs(string itemid, string mid, string groupid, string subgroupid)
        {
            var page_list = Request["page_list"].IsNullorEmpty() ? "" : Request["page_list"];
            var pindex = Request["pindex"].IsNullorEmpty() ? "1" : Request["pindex"];
            var maxpage = Request["maxpage"].IsNullorEmpty() ? "999" : Request["maxpage"];
            var GroupId = "";
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (Session["ArticleIndexNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["ArticleIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("ArticleIndexNoJsSearch");
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
                return RedirectToAction("Index", "Home");
            }

            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            var unitmodel = _IModelArticleManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ViewBag.IntroductionHtml = unitmodel.IntroductionHtml;
            ArticleFrontIndexModel model = new ArticleFrontIndexModel();
            _MasterPageManager.SetModel<ArticleFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelArticleManager.Where(new ModelArticleMain() { ID = int.Parse(itemid) });
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];

            if (mainmodel.Count() == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            var menutype = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];

            if (menu != null)
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
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, mainmodel.First().Name, sitemenuid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.GroupList = _IModelArticleManager.GetGroupSelectList(itemid);
            model.GroupList.First().Text = Common.GetLangText("全部");
            model.Hasgroup = model.GroupList.Count() == 1 ? false : true;
            if (model.Hasgroup)
            {
                model.GroupList.Insert(1, new System.Web.Mvc.SelectListItem() { Text = Common.GetLangText("無分類"), Value = "0" });
            }
            model.MainID = itemid;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            var idx = 0;
            if (unitmodel.Summary.IsNullorEmpty())
            {
                model.tablesummary = "此為" + model.Title + Common.GetLangText("列表") + "，";
                idx = 0;
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    idx += 1;
                    if (c.Name == "序號") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["序號"] + "，"); }
                    if (c.Name == "發佈日期") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["發佈日期"] + "，"); }
                    if (c.Name == "標題") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["標題"] + "，"); }
                    if (c.Name == "類別") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["類別"] + "，"); }
                    if (c.Name == "相關連結") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["相關連結"] + "，"); }
                    if (c.Name == "檔案下載") { model.tablesummary += ("表格欄位" + idx + "為" + model.ColumnNameMapping["檔案下載"] + "，"); }
                }
                model.tablesummary = model.tablesummary.TrimEnd('，');
            }
            else
            {
                model.tablesummary = unitmodel.Summary;
            }
            var limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value;
            var offset = ((page - 1) * limit);
            var searchmodel = new ArticleSearchModel()
            {
                Sort = "Sort",
                Search = "Y",
                Limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value,
                ModelID = int.Parse(model.MainID),
                MenuId = model.MenuID,
                Offset = offset,
                GroupId = groupid.IsNullorEmpty() ? "" : groupid,
                SubGroupId = subgroupid.IsNullorEmpty() ? "" : subgroupid,
                DisplayFrom = DisplayFrom,
                DisplayTo = DisplayTo,
                Title = keyword
            };
            var data = _IModelArticleManager.PagingItemForWebSite(searchmodel.ModelID.ToString(), searchmodel, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);

            idx = offset;
            foreach (var _d in data.rows)
            {
                sb.Append("<tr>");
                idx += 1;
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    if (c.Name == "序號") { sb.Append("<td class='text-center'>" + idx + "</td>"); }
                    if (c.Name == "發佈日期") { sb.Append("<td class='text-center'>" + _d.PublicshDate + "</td>"); }

                    if (c.Name == "標題")
                    {

                        if (model.MenuID != "-1")
                        {
                            sb.Append("<td><a href='" + Url.Action("ArticleView", new { id = _d.ItemID, mid = model.MenuID, page = searchmodel.NowPage, groupid = searchmodel.GroupId, subgroupid }) +
                                "' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                        }
                        else
                        {
                            sb.Append("<td><a href='" + Url.Action("ArticleView", new { id = _d.ItemID, page = searchmodel.NowPage, groupid = searchmodel.GroupId, subgroupid }) +
                              "' title='" + _d.Title + "'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</a></td>");
                        }
                    }
                    if (c.Name == "類別") { sb.Append("<td class='text-center'>" + (_d.GroupName == null ? "" : _d.GroupName) + "</td>"); }
                    if (c.Name == "相關連結")
                    {
                        if (_d.LinkUrl.IsNullorEmpty())
                        {
                            sb.Append("<td class='text-center'></td>");
                        }
                        else
                        {
                            sb.Append("<td class='text-center'>");
                            sb.Append("<a href = '" + _d.LinkUrl + "' title='相關連結-" + _d.LinkUrlDesc + "(另開新視窗)'  target='_blank'>");
                            sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true' aria-label='前往其他頁面'>" +
                                "<i class='fa fa-square fa-stack-2x font-blue-steel' aria-label='前往其他頁面' aria-hidden='true'></i>" +
                                "<i class='fas fa-link fa-stack-1x' aria-label='前往其他頁面' aria-hidden='true'></i>");
                            sb.Append("<span class='sr-only'>前往其他頁面</span></span></a></td>");
                        }
                    }
                    if (c.Name == "檔案下載")
                    {
                        if (_d.UploadFileName.IsNullorEmpty())
                        {
                            sb.Append("<td class='text-center'></td>");
                        }
                        else
                        {
                            sb.Append("<td class='text-center'>");
                            sb.Append("<a href = '" + Url.Action("FileDownLoad", new { itemid = _d.ItemID }) + "'aria-label='前往其他頁面' title='檔案下載-" + _d.FileUrlDesc + ".pdf(另開新視窗)'  target='_blank'>");
                            sb.Append("<span class='fa-stack fa-1g'  aria-hidden='true'>" +
                                "<i class='fa fa-square fa-stack-2x font-blue-steel' aria-label='前往其他頁面' aria-hidden='true'></i>" +
                                "<i class='fas fa-download fa-stack-1x' aria-label='前往其他頁面' aria-hidden='true'></i>");
                            sb.Append("<span class='sr-only'>前往其他頁面</span></span></a></td>");
                        }

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
            if (data.total == 0 || unitmodel.ShowCount == -1) { ViewBag.showpagenum = "N"; }
            else
            {
                ViewBag.maxpage = pagecnt;
            }

            return View(model);
        }
        #endregion
    }
}