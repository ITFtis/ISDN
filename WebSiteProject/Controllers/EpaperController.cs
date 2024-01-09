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
using System.ComponentModel.DataAnnotations;

namespace WebSiteProject.Controllers
{
    public class EpaperController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelEventListManager _IModelEventListManager;
        IMenuManager _IMenuManager;
        IEPaperManager _IEPaperManager;
        public EpaperController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IMenuManager = serviceinstance.MenuManager;
            _IModelEventListManager = serviceinstance.ModelEventListManager;
            _IEPaperManager = serviceinstance.EPaperManager;
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            //if (Request.HttpMethod.ToLower() == "post")
            //{
            //    System.Web.Helpers.AntiForgery.Validate();
            //}
            if (Request["btnorder"] != null) {
                var input = Request["txtEmailInput"];
                if (input.IsNullorEmpty())
                {
                    ViewBag.ErrorInfo = "訂閱EMail請確實輸入";
                }
                else {
                    var echeck = new EmailAddressAttribute();
                    if (echeck.IsValid(input) == false)
                    {
                        ViewBag.ErrorInfo = "EMail格式錯誤";
                    }
                    else {
                        ViewBag.ErrorInfo= _IEPaperManager.AddSubscriber(input, "user");
                    }
                }
            }
            else if (Request["btncancel"] != null)
            {
                var input = Request["txtEmailInput"];
                if (input.IsNullorEmpty())
                {
                    ViewBag.ErrorInfo = "取消訂閱EMail請確實輸入";
                }
                else
                {
                    var echeck = new EmailAddressAttribute();
                    if (echeck.IsValid(input) == false)
                    {
                        ViewBag.ErrorInfo = "EMail格式錯誤";
                    }
                    else
                    {
                        ViewBag.ErrorInfo = _IEPaperManager.DelSubscriber(input, "user");
                    }
                }
            }
            var page_list = Request["page_list"].IsNullorEmpty() ? "" : Request["page_list"];
            var pindex = Request["pindex"].IsNullorEmpty() ? "1" : Request["pindex"];
            var maxpage = Request["maxpage"].IsNullorEmpty() ? "999" : Request["maxpage"];
            var GroupId = "";
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (Session["EpaperNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["EpaperNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("EpaperNoJsSearch");
                    }
                }

            }
            ViewBag.GroupId = GroupId;
            ViewBag.DisplayFrom = DisplayFrom;
            ViewBag.DisplayTo = DisplayTo;
            ViewBag.keyword = keyword;
            ViewBag.pagetype = Request["pagetype"].IsNullorEmpty() ? "news_list" : Request["pagetype"];
            ViewBag.year =  Request["year"].IsNullorEmpty() ? "" : Request["year"];
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
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            EPaperFrontIndexModel model = new EPaperFrontIndexModel();
            _MasterPageManager.SetModel<EPaperFrontIndexModel>(ref model, Device, LangID, mid);
            var unitmodel = _IEPaperManager.GetUnitModel("1");
            if (menu != null)
            {
                //if (menu.ID == 0) { return RedirectToAction("Index", "Home"); }
                model.BannerImage = menu.ImgNameOri.IsNullorEmpty() ? (model.BannerImage == "" ? "fromclass" : model.BannerImage) : menu.ImageUrl;
                model.Title = menu.MenuName;
            }
            else
            {
                model.Title = "電子報";
            }
            var sitemenuid = string.IsNullOrEmpty(Request["sitemenuid"]) ? "-1" : Request["sitemenuid"];
            model.LinkStr = _MasterPageManager.GetFrontLinkString(itemid, mid, model.Title, sitemenuid);
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.MaxTableCount = unitmodel.ShowCount.Value.ToString();
            ViewBag.yeargroup = _IEPaperManager.GetEPaperYearstr(ViewBag.year);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", LangID, model.Title);
            model.MaxTableCount = unitmodel.ShowCount.ToString();
            model.ColumnNameMapping = unitmodel.ColumnNameMapping;
            model.ColumnSetting = unitmodel.UnitSettingColumnList;
            model.tablesummary = unitmodel.Summary;
            var limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value;
            var offset = ((page - 1) * limit);
            var searchmodel = new SearchModelBase()
            {
                Sort = "Sort",
                Search = "Y",
                Limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value,
                Key = ViewBag.year,
                Offset = offset,
                Status="1"
            };
            var data = _IEPaperManager.PagingAdminItem( searchmodel);
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            var ColumnSetting = unitmodel.UnitSettingColumnList;
            var idx = offset;
            foreach (var _d in data.rows)
            {
                sb.Append("<tr>");
                idx += 1;
                foreach (var c in ColumnSetting)
                {
                    if (c.Sellected == 0) { continue; }
                    if (c.Name == "序號") { sb.Append("<td class='text-center'>" + idx + "</td>"); }
                    if (c.Name == "發佈日期") { sb.Append("<td class='text-center'>" + _d.PublicshStr + "</td>"); }
                    if (c.Name == "電子報名稱"){sb.Append("<td><a href='" + Url.Action("EPaperReview", new { id = _d.ID }) +"' title='" + _d.Title + ".pdf(另開新視窗)'  target='_blank'>" + _d.Title + "</a></td>");}
                    if (c.Name == "年份") { sb.Append("<td class='text-center'>" + _d .Year+ "</td>"); }
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
            model.MenuID = mid;
            return View(model);
        }
        #endregion

        #region IndexNoJsSearch
        public ActionResult IndexNoJsSearch(string itemid, string mid)
        {
            //if (Request.HttpMethod.ToLower() == "post")
            //{
            //    System.Web.Helpers.AntiForgery.Validate();
            //}
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
                Session["EpaperNoJsSearch"] = skey;
            }
            else
            {
                if (Session["EpaperNoJsSearch"] != null) { Session.Remove("EpaperNoJsSearch"); }
            }
            return RedirectToAction("Index", "EPaper", new { itemid = itemid, mid = mid });
        }
        #endregion
        public ActionResult EPaperReview(string id)
        {
            var model = new EPaperEditModel();
            if (id != "-1")
            {
                model = _IEPaperManager.GetModel(id);
                model.EPaperItemEdit = _IEPaperManager.GetEPaperItemEdit(id);
            }
            return View(model);
        }
    }
}