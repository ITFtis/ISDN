using Services.Interface;
using Services.Manager;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSiteProject.Code;
using ViewModels;
using Utilities;

namespace WebSiteProject.Controllers
{
    public class SearchController : AppController
    {
        MasterPageManager _IMasterPageManager;
        IModelManager _IModelManager;
        ISearchManager _SearchManager;
        IMenuManager _IMenuManager;
        public SearchController()
        {
            _IMasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelManager = new ModelManager(connectionstr);
            _SearchManager=new SearchManager(connectionstr);
            _IMenuManager = serviceinstance.MenuManager;
        }

        #region Search
        public ActionResult Search(string key, string key2, string key3, string sel1, string sel2, string sellimit
            , string searchtype,string menutype,string menu1, string menu2, string menu3)
        {
            //if (Request.HttpMethod.ToLower() == "post")
            //{
            //    System.Web.Helpers.AntiForgery.Validate();
            //}
            if (this.IsNojavascript) { return RedirectToAction("SearchNoJs", new { key = key, menutype= menutype }); }
            var langid = _IMasterPageManager.CheckLangID("");
            //_SearchManager.SetKeyCount(key, langid);
            ViewBag.langid = langid;
            var model = new MasterPageModel();
            _IMasterPageManager.SetModel<MasterPageModel>(ref model, Device, LangID, "");
            if (model.IsNoJs == 1) { return RedirectToAction("SearchNoJs", new { key = key, menutype = menutype }); }
            if (string.IsNullOrEmpty(key))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Key = key;
            ViewBag.Key2 = key2 == null ? "" : key2;
            ViewBag.Key3 = key3 == null ? "" : key3;
            ViewBag.Sel1 = sel1 == null ? "" : sel1;
            ViewBag.Sel2 = sel2 == null ? "" : sel2;
            ViewBag.SearchType = searchtype == null ? "1" : searchtype;
            //ViewBag.SelLimit = sellimit == null ? "10" : sellimit;

            ViewBag.MenuType = menutype == null ? "" : menutype;
            ViewBag.Menu1 = menu1 == null ? "" : menu1;
            ViewBag.Menu2 = menu2 == null ? "" : menu2;
            ViewBag.Menu3 = menu3 == null ? "" : menu3;
            IModelManager _IModelManager = new ModelManager(connectionstr);
            var indexmodel = _IModelManager.GetPageIndexSettingModel(langid);
            ViewBag.SelLimit = indexmodel.ShowCount;
            ViewBag.ColumnNameMapping = indexmodel.ColumnNameMapping;
            model.SEOScript = _IMasterPageManager.GetSEOData("", "", langid, Common.GetLangText("搜尋結果"));
            return View(model);
        }
        #endregion

        #region SearchNoJs
        public ActionResult SearchNoJs(string key, string key2, string key3, string sel1, string sel2, string sellimit
            , string searchtype, string menutype, string menu1, string menu2, string menu3)
        {
            //if (Request.HttpMethod.ToLower() == "post")
            //{
            //    System.Web.Helpers.AntiForgery.Validate();
            //}
            ViewBag.key = key;
            var page_list = Request["page_list"].IsNullorEmpty() ? "" : Request["page_list"];
            var pindex = Request["pindex"].IsNullorEmpty() ? "1" : Request["pindex"];
            var maxpage = Request["maxpage"].IsNullorEmpty() ? "999" : Request["maxpage"];
            var GroupId = "";
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (Session["MessageIndexNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["MessageIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("MessageIndexNoJsSearch");
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

            var langid = _IMasterPageManager.CheckLangID("");
            //_SearchManager.SetKeyCount(key, langid);
            ViewBag.langid = langid;
            var model = new MasterPageModel();
            _IMasterPageManager.SetModel<MasterPageModel>(ref model, Device, LangID, "");
            if (string.IsNullOrEmpty(key))
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Key = key;
            IModelManager _IModelManager = new ModelManager(connectionstr);
            var indexmodel = _IModelManager.GetPageIndexSettingModel(langid);
            ViewBag.SelLimit = indexmodel.ShowCount;
            ViewBag.ColumnNameMapping = indexmodel.ColumnNameMapping;
            model.SEOScript = _IMasterPageManager.GetSEOData("", "", langid, Common.GetLangText("搜尋結果"));
            AdvanceSearchModel searchbase = new AdvanceSearchModel() { };
            searchbase.Search = "";
            searchbase.Sort = "Sort";
            searchbase.Order = "UpdateDatetime";
            searchbase.Limit = indexmodel.ShowCount==null?30 : indexmodel.ShowCount.Value;
            searchbase.NowPage = nowpage;
            searchbase.LangId =this.LangID;
            searchbase.Key = key==null?"": key;
            var data = _SearchManager.Paging(searchbase);
            var sb = new System.Text.StringBuilder();
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            for (var idx = 0; idx < data.rows.Count(); idx++)
            {
                sb.Append("<a href='" + data.rows[idx].Url + "' title='" + data.rows[idx].Title2 + "'><div class='item'><div class='title'>");
                sb.Append(data.rows[idx].Title+ "</div></div></a>");
            }
            ViewBag.Html = sb.ToString();
            decimal pagecnt = -1;
            if (searchbase.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)searchbase.Limit);
            }
            ViewBag.Html = sb.ToString();

            searchbase.Offset = (int)searchbase.Limit * (nowpage - 1);
             var endcnt = (searchbase.Offset + searchbase.Limit) > data.total ? data.total : (searchbase.Offset + searchbase.Limit);
            ViewBag.TotalCntStr = data.total + "， " + Common.GetLangText("顯示") + " : " + (searchbase.Offset+1) + "~" + endcnt;
            ViewBag.maxpage = 0;
            if (data.total == 0 || indexmodel.ShowCount == -1) { ViewBag.showpagenum = "N"; }
            else
            {
                ViewBag.maxpage = pagecnt;
            }
            return View(model);
        }
        #endregion

        #region PaddingSearch
        public ActionResult PaddingSearch(AdvanceSearchModel model)
        {
            return Json(_SearchManager.Paging(model));
        }
        #endregion

        #region GetMenuTypeList
        public ActionResult GetMenuTypeList(string typeid,string level)
        {
            var langid = _IMasterPageManager.CheckLangID("");
            var data = _IMenuManager.GetMenuTypeList(typeid, langid, level);
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>" + Common.GetLangText("不拘") + "</option>");
            var itemarr = new int[] { 0, 1, 2, 3, 4, 7 };
            foreach (var l in data.Where(v=>v.LinkMode <= 2 && itemarr.Contains(v.ModelID) ))
            {
                sb.Append("<option value='" + l.ID.ToString() + "'>" + l.MenuName + "</option>");
            }
            return Json(sb.ToString());
        }
        #endregion

        #region GetMenuIDList
        public ActionResult GetMenuIDList(string menuid)
        {
            var langid = _IMasterPageManager.CheckLangID(menuid);
            var data = _IMenuManager.GetMenuIDList("999");
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>"+ Common.GetLangText("不拘") +"</option>");
            var itemarr = new int[] { 0, 1, 2, 3, 4, 7 };
            foreach (var l in data.Where(v => v.LinkMode <=2 && itemarr.Contains(v.ModelID)))
            {
                sb.Append("<option value='" + l.ID.ToString() + "'>" + l.MenuName + "</option>");
            }

            return Json(sb.ToString());
        }
        #endregion
        
    }
}