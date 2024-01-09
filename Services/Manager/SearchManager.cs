using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using System.Web.Mvc;
using ViewModels;
using SQLModel;
using Utilities;
using System.Text.RegularExpressions;
using System.Web;

namespace Services.Manager
{
    public class SearchManager : ISearchManager
    {
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<MenuMessageItem> _MessageItemrepository;
        readonly SQLRepository<CountSearchKey> _SKrepository;
        public SearchManager(string connectionstr)
        {
            _menusqlrepository =new SQLRepository<Menu>(connectionstr);
            _MessageItemrepository = new SQLRepository<MenuMessageItem>(connectionstr);
            _SKrepository = new SQLRepository<CountSearchKey>(connectionstr);
        }

        #region Paging
        public PagingInfo<SearchResult> Paging(AdvanceSearchModel model)
        {
            var allmenu = _menusqlrepository.GetByWhere("LangID=@1 and ModelID In (0,1,2,3,4,7)", new object[] { model.LangId });
            //移除第二層是空白的
            var l1menu = allmenu.Where(v => v.MenuLevel == 1 && v.Status == true);
            // var l2menu = allmenu.Where(v => v.MenuLevel ==2 && l1menu.Any( x=>x.ID==v.ParentID));
            var l2menu = allmenu.Where(v => v.MenuLevel == 2 && v.Status == true);
            var l3menu = allmenu.Where(v => v.MenuLevel == 3 && v.Status == true);
            // var l3menu = allmenu.Where(v => v.MenuLevel == 3 && l2menu.Any(x => x.ID == v.ParentID));
            allmenu = l1menu.Union(l2menu).Union(l3menu);

            var result = new PagingInfo<SearchResult>();
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var qdate = DateTime.Now;
            var startidx = (model.NowPage - 1) * model.Limit;
            var endidx = model.NowPage * model.Limit;
            if (model.Limit == -1)
            {
                startidx = 0;
                endidx = 10000;
            }
            var cntarr =new  List<int>();
            MatchEvaluator evaluator = new MatchEvaluator(SetSpanText);
            var pagecnt = 0;
            if (model.Info.IsNullorEmpty()==false) {
                cntarr = model.Info.Split(',').Select(v => int.Parse(v)).ToList();
            }

            var searchKey = model.Key;
            var columnName = "PageIndexItem.*,Menu.ID as MenuID";
           
            //搜尋Page
            var qstrtype = " and Menu.MenuType<=3";



            var qstr = "Menu.LangID=@1 and PageIndexItem.Lang_ID=@1 and Enabled=1 and IsVerift=1 and " +
            "(Menu.ModelItemID = PageIndexItem.ModelID) and Menu.ModelID=1"+ qstrtype+" Order By Sort";


            List<MenuMessageItem> page = new List<MenuMessageItem>();
            var menucheckid = new List<int>();
            var allmenucheckid = allmenu.Select(v => v.ID).ToList();
         
            var iscontinue = false;
            if (model.Info.IsNullorEmpty()==false) { pagecnt = cntarr[0]; }
               
            if (model.Info.IsNullorEmpty() == false)
            {
                if (startidx <= pagecnt) {
                    page = _MessageItemrepository.GetByWhere(qstr, "PageIndexItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
                    page = page.Where(v => allmenucheckid.Contains(v.MenuID)).ToList();
                    page = SetMenuMessageItem(page, model.Key,"", "", "", "", "");
                }
            }
            else {
                page = _MessageItemrepository.GetByWhere(qstr, "PageIndexItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
                page = page.Where(v => allmenucheckid.Contains(v.MenuID)).ToList();
                page = SetMenuMessageItem(page, model.Key, "", "", "", "", "");
                result.total += page.Count();
                pagecnt = result.total;
                if (model.TotalCnt == 0) { cntarr.Add(page.Count()); }
            }
            var text = "";
            if (startidx < 0) { startidx = 0; }
            for (var idx = startidx; idx < endidx; idx++) {
                if (idx >= pagecnt) { continue; }
                var m = page[idx];
                m.HtmlContent = m.HtmlContent.IsNullorEmpty() ? "" : Regex.Replace(m.HtmlContent == null ? "" : m.HtmlContent, "<.*?>", String.Empty);
                text = getText(m, model.Key, "", "", "", evaluator, searchKey);
                result.rows.Add(new SearchResult()
                {
                    ItemID = m.ItemID,
                    ModelID = m.ModelID,
                    Title= Regex.Replace(m.ItemName == null ? "" : m.ItemName, searchKey, evaluator, RegexOptions.None),
                   Title2 = m.ItemName,
                   Text = text.Length > 200 ? text.Substring(0, 200) : text,
                    Url = helper.Action("Index", "Page") + "?itemid=" + m.ModelID + "&mid=" + m.MenuID + "&pageitemid=" + m.ItemID
                });
            }

            List<MenuMessageItem> message = new List<MenuMessageItem>();
            columnName = "MessageItem.*,MessageItem.Title as ItemName,Menu.ID as MenuID";
            qstr = "Menu.LangID=@1 and MessageItem.Lang_ID=@1 and  Enabled=1  and IsVerift=1 and ((StDate <=@2 or StDate is null or StDate='') and  (EdDate >=@2 or EdDate is null or EdDate='')) and " +
                "(Menu.ModelItemID = MessageItem.ModelID) and Menu.ModelID=2 "+ qstrtype+" Order By Sort";

            if (model.Info.IsNullorEmpty() == false)
            {
                if (pagecnt < endidx && (pagecnt + cntarr[1]) >= startidx)
                {
                    iscontinue = true;
                }
                if (iscontinue)
                {
                    message = _MessageItemrepository.GetByWhere(qstr, "MessageItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
                    message = message.Where(v => allmenucheckid.Contains(v.MenuID)).ToList();
                    message = SetMenuMessageItem(message, model.Key, "", "", "", "", "");

                }
            }
            else
            {
                message = _MessageItemrepository.GetByWhere(qstr, "MessageItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
                message = message.Where(v => allmenucheckid.Contains(v.MenuID)).ToList();
                message = SetMenuMessageItem(message, model.Key, "", "", "", "", "");
                if (model.TotalCnt == 0) { cntarr.Add(message.Count()); }
                result.total += message.Count();
                if (pagecnt < endidx && (pagecnt + cntarr[1]) >= startidx)
                {
                    iscontinue = true;
                }
            }
            if (iscontinue)
            {
                for (var idx = 0; idx < message.Count(); idx++)
                {
                    if (pagecnt >= endidx) { continue; }
                    if ((pagecnt) < startidx) { pagecnt += 1; continue; }
                    pagecnt += 1;
                    var m = message[idx];
                    m.HtmlContent = m.HtmlContent.IsNullorEmpty() ? "" : Regex.Replace(m.HtmlContent == null ? "" : m.HtmlContent, "<.*?>", String.Empty);
                    text = getText(m, model.Key, "", "", "", evaluator, searchKey);
                    result.rows.Add(new SearchResult()
                    {
                        ItemID = m.ItemID,
                        ModelID = m.ModelID,
                        Title2 = m.ItemName,
                        Title = Regex.Replace(m.ItemName == null ? "" : m.ItemName, searchKey, evaluator, RegexOptions.None),
                        Text = text.Length > 200 ? text.Substring(0, 200) : text,
                        Url = helper.Action("MessageView", "Message") + "?id=" + m.ItemID + "&mid=" + m.MenuID
                    });
                }
            }

            ////搜尋 active
            if (model.Info.IsNullorEmpty() == false) { pagecnt = (cntarr[0] + cntarr[1]); } else {
                pagecnt = result.total;
            }
            List<MenuMessageItem> active = new List<MenuMessageItem>();
            columnName = "ActiveItem.*,ActiveItem.Title as ItemName,Menu.ID as MenuID";
            qstr = "Menu.LangID=@1 and ActiveItem.Lang_ID=@1 and  Enabled=1 and IsVerift=1 and ((StDate <=@2 or StDate is null or StDate='') and  (EdDate >=@2 or EdDate is null or EdDate='')) and " +
                "(Menu.ModelItemID = ActiveItem.ModelID) and Menu.ModelID=3 "+ qstrtype+" Order By Sort";

            if (model.Info.IsNullorEmpty() == false)
            {
                if (pagecnt < endidx && (pagecnt + cntarr[2]) >= startidx)
                {
                    iscontinue = true;
                }
                if (iscontinue)
                {
                    active = _MessageItemrepository.GetByWhere(qstr, "ActiveItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
                    active = active.Where(v => allmenucheckid.Contains(v.MenuID)).ToList();
                    active = SetMenuMessageItem(active, model.Key, "", "", "", "", "");
                }
            }
            else
            {
                active = _MessageItemrepository.GetByWhere(qstr, "ActiveItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
                active = active.Where(v => allmenucheckid.Contains(v.MenuID)).ToList();
                active = SetMenuMessageItem(active, model.Key, "", "", "", "", "");
                if (model.TotalCnt == 0) { cntarr.Add(active.Count()); }
                result.total += active.Count();
                if (pagecnt < endidx && (pagecnt + cntarr[2]) >= startidx)
                {
                    iscontinue = true;
                }
            }
            if (iscontinue)
            {
                for (var idx = 0; idx < active.Count(); idx++)
                {
                    if (pagecnt >= endidx) { continue; }
                    if ((pagecnt) < startidx) { pagecnt += 1; continue; }
                    pagecnt += 1;
                    var m = active[idx];
                    m.HtmlContent = m.HtmlContent.IsNullorEmpty() ? "" : Regex.Replace(m.HtmlContent == null ? "" : m.HtmlContent, "<.*?>", String.Empty);
                    text = getText(m, model.Key, "", "", "", evaluator, searchKey);
                    result.rows.Add(new SearchResult()
                    {
                        ItemID = m.ItemID,
                        ModelID = m.ModelID,
                        Title = Regex.Replace(m.ItemName == null ? "" : m.ItemName, searchKey, evaluator, RegexOptions.None),
                        Title2 = m.ItemName,
                        Text = text.Length > 200 ? text.Substring(0, 200) : text,
                        Url = helper.Action("ActiveView", "Active") + "?id=" + m.ItemID + "&mid=" + m.MenuID
                    });
                }
            }

            ////搜尋 Video
            //if (model.Info.IsNullorEmpty() == false) { pagecnt = (cntarr[0] + cntarr[1] + cntarr[2]); }else{pagecnt = result.total;}
            //List<MenuMessageItem> download = new List<MenuMessageItem>();
            //columnName = "VideoItem.*,VideoItem.Title as ItemName,Menu.ID as MenuID";
            //qstr = "Menu.LangID=@1 and VideoItem.Lang_ID=@1 and  Enabled=1 and IsVerift=1 and ((StDate <=@2 or StDate is null or StDate='') and  (EdDate >=@2 or EdDate is null or EdDate='')) and " +
            //    "(Menu.ModelItemID = VideoItem.ModelID) and Menu.ModelID=18  " + qstrtype + " Order By Sort"; ;

            //if (model.Info.IsNullorEmpty() == false)
            //{
            //    if (pagecnt < endidx && (pagecnt + cntarr[3]) >= startidx)
            //    {
            //        iscontinue = true;
            //    }
            //    if (iscontinue)
            //    {
            //        download = _MessageItemrepository.GetByWhere(qstr, "VideoItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
            //        if (model.Menu1 != "")
            //        {
            //            download = download.Where(v => menucheckid.Contains(v.MenuID)).ToList();
            //        }
            //        else { download = download.Where(v => allmenucheckid.Contains(v.MenuID)).ToList(); }
            //        download = SetMenuMessageItem(download, model.Key, model.Key2, model.Key3, model.Sel1, model.Sel2, model.SearchType);
            //    }
            //}
            //else
            //{
            //    download = _MessageItemrepository.GetByWhere(qstr, "VideoItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
            //    if (model.Menu1 != "")
            //    {
            //        download = download.Where(v => menucheckid.Contains(v.MenuID)).ToList();
            //    }
            //    else { download = download.Where(v => allmenucheckid.Contains(v.MenuID)).ToList(); }
            //    download = SetMenuMessageItem(download, model.Key, model.Key2, model.Key3, model.Sel1, model.Sel2, model.SearchType);
            //    if (model.TotalCnt == 0) { cntarr.Add(download.Count()); }
            //    result.total += download.Count();
            //    if (pagecnt < endidx && (pagecnt + cntarr[3]) >= startidx)
            //    {
            //        iscontinue = true;
            //    }
            //}
            //if (iscontinue)
            //{
            //    for (var idx = 0; idx < download.Count(); idx++)
            //    {
            //        if (pagecnt >= endidx) { continue; }
            //        if ((pagecnt) < startidx) { pagecnt += 1; continue; }
            //        pagecnt += 1;
            //        var m = download[idx];
            //        m.HtmlContent = m.HtmlContent.IsNullorEmpty() ? "" : Regex.Replace(m.HtmlContent == null ? "" : m.HtmlContent, "<.*?>", String.Empty);
            //        text = getText(m, model.Key, model.Key2, model.Key3, model.SearchType, evaluator, searchKey);
            //        result.rows.Add(new SearchResult()
            //        {
            //            ItemID = m.ItemID,
            //            ModelID = m.ModelID,
            //            Title=m.ItemName,
            //            Text = text.Length > 200 ? text.Substring(0, 200) : text,
            //            Url = helper.Action("Index", "Download") + "?itemid=" + m.ModelID + "&mid=" + m.MenuID
            //        });
            //    }
            //}

      
            //if (model.Info.IsNullorEmpty() == false) { pagecnt = (cntarr[0] + cntarr[1] + cntarr[2] + cntarr[3]); } else { pagecnt = result.total; }
            //List<MenuMessageItem> article = new List<MenuMessageItem>();
            //columnName = "MessageItem.*,MessageItem.Title as ItemName,Menu.ID as MenuID";
            //qstr = "Menu.LangID=@1 and MessageItem.Lang_ID=@1 and  Enabled=1 and ((StDate <=@2 or StDate is null or StDate='') and  (EdDate >=@2 or EdDate is null or EdDate='')) and " +
            //    "(Menu.ModelItemID = MessageItem.ModelID) and Menu.ModelID=2 " + qstrtype + " Order By Sort"; ; 

            //if (model.Info.IsNullorEmpty() == false)
            //{
            //    if (pagecnt < endidx && (pagecnt + cntarr[4]) >= startidx)
            //    {
            //        iscontinue = true;
            //    }
            //    if (iscontinue)
            //    {
            //        article = _MessageItemrepository.GetByWhere(qstr, "MessageItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
            //        if (model.Menu1 != "")
            //        {
            //            article = article.Where(v => menucheckid.Contains(v.MenuID)).ToList();
            //        }
            //        article = SetMenuMessageItem(article, model.Key, model.Key2, model.Key3, model.Sel1, model.Sel2, model.SearchType);
            //    }
            //}
            //else
            //{
            //    article = _MessageItemrepository.GetByWhere(qstr, "MessageItem,Menu", new object[] { model.LangId, qdate }, columnName).ToList();
            //    if (model.Menu1 != "")
            //    {
            //        article = article.Where(v => menucheckid.Contains(v.MenuID)).ToList();
            //    }
            //    article = SetMenuMessageItem(article, model.Key, model.Key2, model.Key3, model.Sel1, model.Sel2, model.SearchType);
            //    if (model.TotalCnt == 0) { cntarr.Add(article.Count()); }
            //    pagecnt = result.total;
            //    if (pagecnt < endidx && (pagecnt + cntarr[4]) >= startidx)
            //    {
            //        iscontinue = true;
            //    }
            //}
            //if (iscontinue)
            //{
            //    for (var idx = 0; idx < article.Count(); idx++)
            //    {
            //        if (pagecnt >= endidx) { continue; }
            //        if ((pagecnt) < startidx) { pagecnt += 1; continue; }
            //        pagecnt += 1;
            //        var m = article[idx];
            //        m.HtmlContent = m.HtmlContent.IsNullorEmpty() ? "" : Regex.Replace(m.HtmlContent == null ? "" : m.HtmlContent, "<.*?>", String.Empty);
            //        text = getText(m, model.Key, model.Key2, model.Key3, model.SearchType, evaluator, searchKey);
            //        result.rows.Add(new SearchResult()
            //        {
            //            ItemID = m.ItemID,
            //            ModelID = m.ModelID,
            //            Text = text.Length > 200 ? text.Substring(0, 200) : text,
            //            Url = helper.Action("ArticleView", "Article") + "?id=" + m.ItemID + "&mid=" + m.MenuID
            //        });
            //    }
            //}

          
            //if (iscontinue)
            //{
            //    var kmenu = _menusqlrepository.GetByWhere("LangID=@1 and ModelID=8  and Menu.MenuType<=3 and LinkMode=2 and Status=1", new object[] { model.LangId });
            //    //Knowledge/Detail?id=1006&itemid=1&mid=52
            //    var mstr = "";
            //    if (kmenu.Count() > 0) {
            //        mstr = "&itemid=" + kmenu.First().ModelItemID + "&mid=" + kmenu.First().ID;
            //    }
            //    for (var idx = 0; idx < knowledgelist.Count(); idx++)
            //    {
            //        if (pagecnt >= endidx) { continue; }
            //        if ((pagecnt) < startidx) { pagecnt += 1; continue; }
            //        pagecnt += 1;
            //        var m = knowledgelist[idx];
            //        m.Introduction = m.Introduction.IsNullorEmpty() ? "" : Regex.Replace(m.Introduction == null ? "" : m.Introduction, "<.*?>", String.Empty);
            //        m.BookName = m.BookName.IsNullorEmpty() ? "" : Regex.Replace(m.BookName == null ? "" : m.BookName, "<.*?>", String.Empty);
            //        text = m.Introduction.IndexOf(model.Key) >= 0 ? Regex.Replace(m.Introduction == null ? "" : m.Introduction, model.Key, evaluator, RegexOptions.IgnoreCase) :
            //            Regex.Replace(m.BookName == null ? "" : m.BookName, model.Key, evaluator, RegexOptions.None);
            //        result.rows.Add(new SearchResult()
            //        {
            //            ItemID = m.ID,
            //            ModelID = 0,
            //            Text = text.Length > 200 ? text.Substring(0, 200) : text,
            //            Url = helper.Action("Detail", "Knowledge") + "?id=" + m.ID + mstr
            //        });
            //    }
            //}

            if (model.Info.IsNullorEmpty()==false)
            {

                result.total = model.TotalCnt;
                result.Info = model.Info;
            }
            else {
                //result.rows = result.rows.Skip(startidx).Take(model.Limit).ToList();
                result.Info = string.Join(",", cntarr);
            }
            return result;
        }
        #endregion

        public static string SetSpanText(Match match)
        {
            return "<span class='red'>" + match.Value + "</span>";
        }


        public string getText(MenuMessageItem m,string key1,string key2,string key3,string SearchType, MatchEvaluator evaluator,string searchkey) {
            var text = "";
            var isinHtmlContent = false;
            if (key2 != "" && key3 == "")
            {
                isinHtmlContent = (m.HtmlContent.IndexOf(key1) >= 0 || m.HtmlContent.IndexOf(key2) >= 0) ;
            }
            else if (key2 != "" && key3 != "")
            {
                isinHtmlContent = (m.HtmlContent.IndexOf(key1) >= 0 || m.HtmlContent.IndexOf(key2) >= 0 || m.HtmlContent.IndexOf(key3) >= 0);
            }
            else
            {
                isinHtmlContent = m.HtmlContent.IndexOf(key1) >= 0;
            }
            if (SearchType == "2")
            {
                text = Regex.Replace(m.ItemName == null ? "" : m.ItemName, searchkey, evaluator, RegexOptions.None);
            }
            else if (SearchType == "3")
            {
                text = Regex.Replace(m.HtmlContent == null ? "" : m.HtmlContent, searchkey, evaluator, RegexOptions.None);
            }
            else
            {
                text = isinHtmlContent ? Regex.Replace(m.HtmlContent == null ? "" : m.HtmlContent, searchkey, evaluator, RegexOptions.None) :
               Regex.Replace(m.ItemName == null ? "" : m.ItemName, searchkey, evaluator, RegexOptions.None);
            }
            return text;
        }

        #region SetMenuMessageItem
        public List<MenuMessageItem> SetMenuMessageItem(List<MenuMessageItem> source,
           string key1, string key2, string key3, string sel1, string sel2, string SearchType)
        {
            List<MenuMessageItem> k2 = new List<MenuMessageItem>();
            List<MenuMessageItem> k3 = new List<MenuMessageItem>();
            List<MenuMessageItem> returnlist = new List<MenuMessageItem>();
            if (SearchType == "2")
            {
                returnlist = source.Where(v => v.ItemName.IndexOf(key1) >= 0).ToList();
            }
            else if (SearchType == "3") {
                returnlist = source.Where(v => Regex.Replace(v.HtmlContent == null ? "" : v.HtmlContent, "<.*?>", String.Empty).IndexOf(key1) >= 0).ToList();
            }
            else {
                returnlist = source.Where(v => Regex.Replace(v.HtmlContent == null ? "" : v.HtmlContent, "<.*?>", String.Empty).IndexOf(key1) >= 0|| v.ItemName.IndexOf(key1) >= 0).ToList();
            }
            if (key2 != "")
            {
                if (SearchType == "2")
                {
                    k2 = source.Where(v =>v.ItemName.IndexOf(key2) >= 0).ToList();
                }
                else if (SearchType == "3")
                {
                    k2 = source.Where(v => Regex.Replace(v.HtmlContent == null ? "" : v.HtmlContent, "<.*?>", String.Empty).IndexOf(key2) >= 0).ToList();
                }
                else
                {
                    k2 = source.Where(v => Regex.Replace(v.HtmlContent == null ? "" : v.HtmlContent, "<.*?>", String.Empty).IndexOf(key2) >= 0 || v.ItemName.IndexOf(key2) >= 0).ToList();
                }
            
                if (sel1 == "NOT")
                {
                    returnlist = returnlist.Except(k2).ToList();
                }
                else if (sel1 == "AND")
                {
                    returnlist = returnlist.Intersect(k2).ToList();
                }
                else
                {
                    returnlist = returnlist.Union(k2).ToList();
                }
            }
            if (key3 != "")
            {
                if (SearchType == "2")
                {
                    k3 = source.Where(v =>  v.ItemName.IndexOf(key3) >= 0).ToList();
                }
                else if (SearchType == "3")
                {
                    k3 = source.Where(v => Regex.Replace(v.HtmlContent == null ? "" : v.HtmlContent, "<.*?>", String.Empty).IndexOf(key3) >= 0 ).ToList();
                }
                else
                {
                    k3 = source.Where(v => Regex.Replace(v.HtmlContent == null ? "" : v.HtmlContent, "<.*?>", String.Empty).IndexOf(key3) >= 0 || v.ItemName.IndexOf(key3) >= 0).ToList();
                }
              
                if (sel2 == "NOT")
                {
                    returnlist = returnlist.Except(k3).ToList();
                }
                else if (sel2 == "AND")
                {
                    returnlist = returnlist.Intersect(k3).ToList();
                }
                else
                {
                    returnlist = returnlist.Union(k3).ToList();
                }
            }
            return returnlist;
        } 
        #endregion

        #region SetKeyCount
        public void SetKeyCount(string key,string langid)
        {
            var hasdata = _SKrepository.GetByWhere("SearchKey=@1 and LangID=@2", new object[] { key, langid });
            if (hasdata.Count() > 0)
            {
                _SKrepository.Update("Count=Count+1", "SearchKey=@1  and LangID=@2", new object[] { key, langid });
            }
            else
            {
                if (langid.IsNullorEmpty() == false) {
                    _SKrepository.Create(new CountSearchKey()
                    {
                        Count = 1,
                        SearchKey = key,
                        LangID = int.Parse(langid)
                    });
                }
            }
        } 
        #endregion


    }
}
