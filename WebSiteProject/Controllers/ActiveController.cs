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
using System.Net.Mail;
using System.Net;
using NAudio.Wave;
using NAudio.Lame;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using Newtonsoft.Json;
using HtmlAgilityPack;

namespace WebSiteProject.Controllers
{
    public class ActiveController : AppController
    {
        MasterPageManager _MasterPageManager;
        IModelWebsiteMapManager _IModelWebsiteMapManager;
        IMenuManager _IMenuManager;
        IModelActiveManager _IModelActiveManager;
        ISiteLayoutManager _ISiteLayoutManager;
        ILoginManager _ILoginManager;
        public ActiveController()
        {
            _MasterPageManager = new MasterPageManager(connectionstr, LangID, Common.GetLangDict());
            _IModelWebsiteMapManager = serviceinstance.ModelWebsiteMapManager;
            _IMenuManager = serviceinstance.MenuManager;
            _IModelActiveManager = serviceinstance.ModelActiveManager;
            _ISiteLayoutManager = serviceinstance.SiteLayoutManager;
            _ILoginManager = serviceinstance.LoginManager;
        }

        #region Index
        [NoCacheAttribute]
        public ActionResult Index(string itemid, string mid)
        {
            if (Session["ActiveIndexNoJsSearch"] != null) { Session.Remove("ActiveIndexNoJsSearch"); }

            if (string.IsNullOrEmpty(itemid))
            {
                return RedirectToAction("Index", "Home");
            }
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
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false)
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            var unitmodel = _IModelActiveManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ActiveFrontIndexModel model = new ActiveFrontIndexModel();
            _MasterPageManager.SetModel<ActiveFrontIndexModel>(ref model, Device, LangID, mid);
            if (model.IsNoJs == 1) { return RedirectToAction("IndexNoJs", new { itemid = itemid, mid = mid }); }
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = int.Parse(itemid) });
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
            model.GroupList = _IModelActiveManager.GetFrontGroupSelectList(itemid);
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
            var btntype = Request["btntype"] == null ? "news_list" : Request["btntype"];
            var pagetype = Request["pagetype"].IsNullorEmpty() ? "news_list" : Request["pagetype"];
            var GroupId = "";
            var DisplayFrom = "";
            var DisplayTo = "";
            var keyword = "";
            if (btntype == "search")
            {
                GroupId = Request["GroupId"].IsNullorEmpty() ? "" : Request["GroupId"];
                DisplayFrom = Request["DisplayFrom"].IsNullorEmpty() ? "" : Request["DisplayFrom"];
                DisplayTo = Request["DisplayTo"].IsNullorEmpty() ? "" : Request["DisplayTo"];
                keyword = Request["keyword"].IsNullorEmpty() ? "" : Request["keyword"];
                var skey = new Dictionary<string, string>();
                if (GroupId != "" || DisplayFrom != "" || DisplayTo != "" || keyword != "")
                {
                    skey.Add("GroupId", GroupId);
                    skey.Add("DisplayFrom", DisplayFrom);
                    skey.Add("DisplayTo", DisplayTo);
                    skey.Add("keyword", keyword);
                    Session["ActiveIndexNoJsSearch"] = skey;
                }
                else
                {
                    if (Session["ActiveIndexNoJsSearch"] != null) { Session.Remove("ActiveIndexNoJsSearch"); }
                }
            }
            else if (btntype.IsNullorEmpty() == false)
            {
                pagetype = btntype;
            }
            ViewBag.pagetype = pagetype;
            var page_list = Request["page_list"].IsNullorEmpty() ? "" : Request["page_list"];
            var pindex = Request["pindex"].IsNullorEmpty() ? "1" : Request["pindex"];
            var maxpage = Request["maxpage"].IsNullorEmpty() ? "999" : Request["maxpage"];

            if (Session["ActiveIndexNoJsSearch"] != null)
            {
                var dict = (Dictionary<string, string>)Session["ActiveIndexNoJsSearch"];
                if (dict != null)
                {
                    GroupId = dict.ContainsKey("GroupId") ? dict["GroupId"] : "";
                    DisplayFrom = dict.ContainsKey("DisplayFrom") ? dict["DisplayFrom"] : "";
                    DisplayTo = dict.ContainsKey("DisplayTo") ? dict["DisplayTo"] : "";
                    keyword = dict.ContainsKey("GroupId") ? dict["keyword"] : "";
                    if (GroupId.IsNullorEmpty() && DisplayFrom.IsNullorEmpty() && DisplayTo.IsNullorEmpty() && keyword.IsNullorEmpty())
                    {
                        Session.Remove("ActiveIndexNoJsSearch");
                    }
                }

            }
            ViewBag.GroupId = GroupId;
            ViewBag.DisplayFrom = DisplayFrom;
            ViewBag.DisplayTo = DisplayTo;
            ViewBag.keyword = keyword;

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
            var unitmodel = _IModelActiveManager.GetUnitModel(itemid);
            ViewBag.ColumnNameMapping = unitmodel.ColumnNameMapping;
            ViewBag.ColumnSetting = unitmodel.UnitSettingColumnList;
            ActiveFrontIndexModel model = new ActiveFrontIndexModel();
            _MasterPageManager.SetModel<ActiveFrontIndexModel>(ref model, Device, LangID, mid);
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = int.Parse(itemid) });
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
            model.GroupList = _IModelActiveManager.GetFrontGroupSelectList(itemid);
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
            var limit = unitmodel.ShowCount == null ? 12 : unitmodel.ShowCount.Value;
            var offset = ((page - 1) * limit);
            var searchmodel = new ActiveSearchModel()
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
            var data = _IModelActiveManager.PagingItemForWebSite(searchmodel.ModelID.ToString(), searchmodel, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows)
            {
                sb.Append("<div class='item'>");
                if (searchmodel.MenuId != "-1")
                {
                    sb.Append("<a href='" + Url.Action("ActiveView", new { id = _d.ItemID, mid = searchmodel.MenuId, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "' data-sr='enter bottom over 1.5s'>");
                }
                else
                {
                    sb.Append("<a href='" + Url.Action("ActiveView", new { id = _d.ItemID, page = searchmodel.NowPage, groupid = searchmodel.GroupId }) + "' title='" + _d.Title + "' data-sr='enter bottom over 1.5s'> ");
                }
                
                if (_d.RelatceImageFileName != "")
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/ActiveItem/" + _d.RelatceImageFileName) + "'  alt='' align='left' >");
                }
                else
                {
                    sb.Append("<img src = '" + baseimg + "' alt=''>");
                }
                var group = _d.GroupName.IsNullorEmpty() ? "" : "<div class='top_class'>" + _d.GroupName + "</div>";
                sb.Append("<div class='date'>" + _d.PublicshDate + group + "</div>");
                sb.Append("<div class='title'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</div>");
                sb.Append("<div class='text-shadow-container'></div>");

                sb.Append("</a></div>");
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
                Session["ActiveIndexNoJsSearch"] = skey;
            }
            else
            {
                if (Session["ActiveIndexNoJsSearch"] != null) { Session.Remove("ActiveIndexNoJsSearch"); }
            }
            _MasterPageManager.SetNoJS(0);
            return RedirectToAction("IndexNoJs", "Active", new { itemid = itemid, mid = mid });
        }
        #endregion

        #region PagingItem
        public ActionResult PagingItem(ActiveSearchModel model)
        {
            model.Sort = "Sort";
            var data = _IModelActiveManager.PagingItemForWebSite(model.ModelID.ToString(), model, "");
            var sb = new System.Text.StringBuilder();
            var baseimg = @Url.Content("~/ContentWebsite/image/logo_400x300.jpg");
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            foreach (var _d in data.rows)
            {
                if (model.MenuId != "-1")
                {
                    sb.Append("<a href='" + Url.Action("ActiveView", new { id = _d.ItemID, mid = model.MenuId, page = model.NowPage, groupid = model.GroupId }) + "' title='" + _d.Title + "' data-sr='enter bottom over 1.5s'>");
                }
                else
                {
                    sb.Append("<a href='" + Url.Action("ActiveView", new { id = _d.ItemID, page = model.NowPage, groupid = model.GroupId }) + "' title='" + _d.Title + "' data-sr='enter bottom over 1.5s'> ");
                }
                sb.Append("<div class='item'>");
                if (_d.RelatceImageFileName != "")
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/ActiveItem/" + _d.RelatceImageFileName) + "'  alt='' align='left' >");
                }
                else
                {
                    sb.Append("<img src = '" + baseimg + "' alt=''>");
                }
                var group = _d.GroupName.IsNullorEmpty() ? "" : "<div class='top_class'>" + _d.GroupName + "</div>";
                sb.Append("<div class='date'>" + _d.PublicshDate + group + "</div>");
                sb.Append("<div class='title'>" + _d.Title + (_d.IsNew ? "<span class='new_icon'>new</span>" : "") + "</div>");
                sb.Append("<div class='text-shadow-container'></div>");

                sb.Append("</div></a>");
            }
            decimal pagecnt = -1;
            if (model.Limit != -1)
            {
                pagecnt = Math.Ceiling((decimal)data.total / (decimal)model.Limit);
            }
            return Json(new string[] { sb.ToString(), data.total.ToString(), pagecnt.ToString() });
        }
        #endregion

        #region ActiveView
        public ActionResult ActiveView(string id, string mid, string page, string groupid)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index", "Home");
            }
            Session["messagepage"] = page == null ? "1" : page;
            Session["messagegroup"] = groupid == null ? "" : groupid;
            var itemmodel = _IModelActiveManager.GetModelItem(id);
            if (itemmodel.ItemID == 0) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.Enabled == false) { return RedirectToAction("Index", "Home"); }
            if (itemmodel.IsVerift == false) { return RedirectToAction("Index", "Home"); }
            Session["messagemodelid"] = itemmodel.ModelID.ToString();

            var isusedate = (itemmodel.StDate == null || itemmodel.StDate <= DateTime.Now) && (itemmodel.EdDate == null || itemmodel.EdDate.Value.AddDays(1) >= DateTime.Now);
            if (isusedate == false) { return RedirectToAction("Index", "Home"); }
            MenuEditModel menu = null;
            if (string.IsNullOrEmpty(mid) == false && mid != "-1")
            {
                menu = _IMenuManager.GetModel(mid);
                LangID = menu.LangID.ToString();
            }
            if (LangID == "0") { LangID = "1"; }
            ActiveFrontViewModel model = new ActiveFrontViewModel();
            _MasterPageManager.SetModel<ActiveFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelActiveManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = itemmodel.ModelID.Value });
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
                model.GroupName = _IModelActiveManager.GetGroupName(itemmodel.GroupID.ToString());
            }
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

            model.IsForward = unitmodel.IsForward;
            model.IsPrint = unitmodel.IsPrint;
            model.IsShare = unitmodel.IsShare;
            model.ImageName = itemmodel.ImageFileName;
            model.ImageFileLocation = itemmodel.ImageFileLocation;
            model.LinkUrlDesc = itemmodel.LinkUrlDesc == null ? "" : itemmodel.LinkUrlDesc;
            model.PublicshDate = itemmodel.PublicshDate.Value.ToString("yyyy.MM.dd");
            model.ImageFileDesc = itemmodel.ImageFileDesc;
            UrlHelper helper = new UrlHelper(Request.RequestContext);
            if (itemmodel.RelateImageFileName.IsNullorEmpty() == false)
            {
                itemmodel.RelateImageFileName = helper.Content("~/UploadImage/MessageItem/" + itemmodel.RelateImageFileName);
                var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = itemmodel.RelateImageFileName, Query = null, };
                model.FBImage = urlBuilder.ToString();
            }
            else
            {
                if (itemmodel.HtmlContent != null && itemmodel.HtmlContent.IndexOf("UploadImage/MessageItem") >= 0)
                {
                    model.FBImage = "";
                }
                else
                {
                    var ImageFileName = helper.Content("~/img/logo_fb.jpg");
                    var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri) { Path = ImageFileName, Query = null, };
                    model.FBImage = urlBuilder.ToString();
                }
            }

            var fbtitle = model.Content.TrimgHtmlTag().Replace("\n", "").Replace("\t", "");
            model.FBTitle = fbtitle.Count() > 80 ? fbtitle.Substring(0, 80) : fbtitle;


            model.Title = itemmodel.Title;
            if (itemmodel.LinkUrl.IsNullorEmpty() == false) { model.LinkUrl = itemmodel.LinkUrl; }
            if (itemmodel.UploadFilePath.IsNullorEmpty() == false)
            {
                model.DownloadID = itemmodel.ItemID.ToString();
                model.DownloadDesc = itemmodel.UploadFileDesc;
            }
            model.ShowModel = _MasterPageManager.GetMenuShowModel(mid);
            model.MenuType = string.IsNullOrEmpty(Request["menutype"]) ? "1" : Request["menutype"];
            model.Organizer = itemmodel.Organizer;
            model.ActiveDateStr = itemmodel.ActiveDate == null ? "" : itemmodel.ActiveDate.Value.ToString("yyyy/MM/dd");
            model.Location = itemmodel.Location;
            model.Info = itemmodel.Info;
            model.Price = itemmodel.Price;
            model.Way = itemmodel.Way;
            model.WebSite = itemmodel.WebSite;
            model.Search = itemmodel.Search;

            model.Contact = itemmodel.Contact;
            model.ContactMail = itemmodel.ContactMail;
            model.ContactFax = itemmodel.ContactFax;
            model.Source = itemmodel.Source;
            model.ActiveFiles = _IModelActiveManager.GetAllActiveFiles(id);
            var activerange = _IModelActiveManager.GetModelDataRange(itemmodel.ItemID.ToString());
            if (activerange.Count() > 0)
            {
                var slist = new List<string>();
                foreach (var rg in activerange)
                {
                    slist.Add(rg.StartDate + " ~ " + rg.EndDate);
                }
                model.ActiveRange = string.Join("、", slist.ToArray());
            }
            var IsActive = false;
            if (itemmodel.StSignDate == null && itemmodel.EdSignDate == null) { IsActive = true; }
            else if (itemmodel.StSignDate != null && itemmodel.EdSignDate == null)
            {
                if (DateTime.Now >= itemmodel.StSignDate)
                {
                    IsActive = true;
                }
            }
            else if (itemmodel.StSignDate == null && itemmodel.EdSignDate != null)
            {
                if (DateTime.Now <= itemmodel.EdSignDate.Value.AddDays(1))
                {
                    IsActive = true;
                }
            }
            else if (itemmodel.StSignDate != null && itemmodel.EdSignDate != null)
            {
                if (DateTime.Now >= itemmodel.StSignDate.Value && DateTime.Now <= itemmodel.EdSignDate.Value.AddDays(1))
                {
                    IsActive = true;
                }
            }
            model.IsActive = IsActive;
            _IModelActiveManager.UpdateClickCount(id);
            return View(model);
        }
        #endregion

        #region FileDownLoad
        public ActionResult FileDownLoad(string fileid)
        {
            var model = _IModelActiveManager.GetActiveFiles(fileid);
            string filepath = "";
            string oldfilename = "";
            if (model.FilePath.IsNullorEmpty() == false)
            {
                //取得檔案名稱
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadFile";
                }
                filepath = uploadfilepath + model.FilePath;
                string filename = System.IO.Path.GetFileName(filepath);
                if (string.IsNullOrEmpty(oldfilename)) { oldfilename = filename; }
                //讀成串流
                Stream iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);
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
            var itemmodel = _IModelActiveManager.GetModelItem(id);
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
            ActiveFrontViewModel model = new ActiveFrontViewModel();
            _MasterPageManager.SetModel<ActiveFrontViewModel>(ref model, Device, LangID, mid);
            model.SEOScript = _MasterPageManager.GetSEOData("", "", itemmodel.ModelID.ToString(), id, LangID, itemmodel.Title);
            var unitmodel = _IModelActiveManager.GetUnitModel(itemmodel.ModelID.ToString());
            model.MainID = itemmodel.ModelID.ToString();
            model.ItemID = id;
            model.MenuID = string.IsNullOrEmpty(mid) ? "-1" : mid.ToString();
            var mainmodel = _IModelActiveManager.Where(new ModelActiveEditMain() { ID = itemmodel.ModelID.Value });
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
                model.GroupName = _IModelActiveManager.GetGroupName(itemmodel.GroupID.ToString());
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
            model.Organizer = itemmodel.Organizer;
            model.ActiveDateStr = itemmodel.ActiveDate == null ? "" : itemmodel.ActiveDate.Value.ToString("yyyy/MM/dd");
            model.Location = itemmodel.Location;
            model.Info = itemmodel.Info;
            model.Price = itemmodel.Price;
            model.Way = itemmodel.Way;
            model.WebSite = itemmodel.WebSite;
            model.Search = itemmodel.Search;

            model.Contact = itemmodel.Contact;
            model.ContactMail = itemmodel.ContactMail;
            model.ContactFax = itemmodel.ContactFax;
            model.Source = itemmodel.Source;
            model.ActiveFiles = _IModelActiveManager.GetAllActiveFiles(id);

            var activerange = _IModelActiveManager.GetModelDataRange(itemmodel.ItemID.ToString());
            if (activerange.Count() > 0)
            {
                var slist = new List<string>();
                foreach (var rg in activerange)
                {
                    slist.Add(rg.StartDate + " ~ " + rg.EndDate);
                }
                model.ActiveRange = string.Join("、", slist.ToArray());
            }
            var IsActive = false;
            if (itemmodel.StSignDate == null && itemmodel.EdSignDate == null) { IsActive = true; }
            else if (itemmodel.StSignDate != null && itemmodel.EdSignDate == null)
            {
                if (DateTime.Now >= itemmodel.StSignDate)
                {
                    IsActive = true;
                }
            }
            else if (itemmodel.StSignDate == null && itemmodel.EdSignDate != null)
            {
                if (DateTime.Now <= itemmodel.EdSignDate.Value.AddDays(1))
                {
                    IsActive = true;
                }
            }
            else if (itemmodel.StSignDate != null && itemmodel.EdSignDate != null)
            {
                if (DateTime.Now >= itemmodel.StSignDate.Value && DateTime.Now <= itemmodel.EdSignDate.Value.AddDays(1))
                {
                    IsActive = true;
                }
            }
            model.IsActive = IsActive;
            return View(model);
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
            ViewBag.itemid = itemid;
            ViewBag.mid = mid;
            ViewBag.menutype = menutype;
            ViewBag.sitemenuid = sitemenuid;
            var unitmodel = _IModelActiveManager.GetUnitModel(itemid);
            var sitemodel = _ISiteLayoutManager.GetSiteLayout(Device, "1");
            ViewBag.FooterString = sitemodel.FowardHtmlContent;
            ViewBag.LogoUrl = sitemodel.FowardImageUrl;

            var itemmodel = _IModelActiveManager.GetModelItem(itemid);
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
                ViewBag.Url = hostUrl + Url.Action("ActiveView", "Active", new { id = itemid, mid = mid, sitemenuid = sitemenuid, menutype = menutype });
            }
            else if (string.IsNullOrEmpty(mid) == false)
            {
                ViewBag.Url = hostUrl + Url.Action("ActiveView", "Active", new { id = itemid, mid = mid });
            }
            else
            {
                ViewBag.Url = hostUrl + Url.Action("ActiveView", "Active", new { id = itemid });
            }
            var imagestrArr = _ILoginManager.GetCaptchImage();
            ViewBag.audio = Url.Action("GetAudio");
            ViewBag.captch = imagestrArr[0];
            ViewBag.captchimg = imagestrArr[1];
            return View();
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
    }
}