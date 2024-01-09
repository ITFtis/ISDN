using Services.Interface;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Utilities;
using ViewModels;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class MenuManager : IMenuManager
    {
        readonly IModelPageEditManager _IPageEditManager;
        readonly IModelMessageManager _IMessageManager;
        readonly IModelFileDownloadManager _IModelFileDownloadManager;
        readonly IModelActiveManager _IModelActiveManager;
        readonly ILangManager _ILangManager;
        readonly IModelFormManager _IModelFormManager;
        readonly IModelEventListManager _IModelEventListManager;
        readonly IModelArticleManager _IModelArticleManager;
        readonly IModelPatentManager _IModelPatentManager;
        readonly SQLRepository<ModelPageEditMain> _pageindexqlrepository;
        readonly SQLRepository<ModelMessageMain> _messageqlrepository;
        readonly SQLRepository<ModelActiveEditMain> _activesqlrepository;
        readonly SQLRepository<ModelWebsiteMapMain> _mapsqlrepository;
        readonly SQLRepository<ModelLangMain> _langsqlrepository;
        readonly SQLRepository<Menu> _sqlrepository;
        readonly SQLRepository<MenuUrl> _urlsqlrepository;
        readonly SQLRepository<ModelFileDownloadMain> _filesqlrepository;
        readonly SQLRepository<ModelFormMain> _formsqlrepository;
        readonly SQLRepository<ModelEventListMain> _eventsqlrepository;
        readonly SQLRepository<ModelArticleMain> _articlesqlrepository;
        readonly SQLRepository<ModelPatentMain> _patentMainsqlrepository;
        public MenuManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.Menu;
            _pageindexqlrepository = sqlinstance.ModelPageEditMain;
            _messageqlrepository= sqlinstance.ModelMessageMain;
            _activesqlrepository = sqlinstance.ModelActiveEditMain;
            _mapsqlrepository = sqlinstance.ModelWebsiteMapMain;
            _langsqlrepository = sqlinstance.ModelLangMain;
            _urlsqlrepository= sqlinstance.MenuUrl;
            _filesqlrepository = sqlinstance.ModelFileDownloadMain;
            _formsqlrepository= sqlinstance.ModelFormMain;
            _eventsqlrepository = sqlinstance.ModelEventListMain;
            _articlesqlrepository = sqlinstance.ModelArticleMain;
            _patentMainsqlrepository = sqlinstance.ModelPatentMain;
            _IPageEditManager = new ModelPageEditManager(sqlinstance);
            _IMessageManager = new ModelMessageManager(sqlinstance);
            _ILangManager = new LangManager(sqlinstance);
            _IModelFileDownloadManager = new ModelFileDownloadManager(sqlinstance);
            _IModelActiveManager = new ModelActiveManager(sqlinstance);
            _IModelFormManager = new ModelFormManager(sqlinstance);
            _IModelEventListManager=new ModelEventListManager(sqlinstance);
            _IModelArticleManager = new ModelArticleManager(sqlinstance);
            _IModelPatentManager=new ModelPatentManager(sqlinstance);
        }

        #region GetMenuTypeList
        public Menu[] GetMenuTypeList(string type,string langid,string level)
        {
            var model = new MenuEditModel();

            var data = _sqlrepository.GetByWhere("LangID=@1 and MenuType=@2 and MenuLevel=@3 and Status=1  order by Sort", new object[] { langid, type, level }).ToArray();
            return data;
        }
        #endregion

        #region GetModelItem
        public string GetModelItem(string modelid, string langid,bool hasnewitem=true)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>無</option>");

            if (modelid == "1")
            {
                if (hasnewitem) { sb.Append("<option value='-1'>新建單元</option>"); } 
                var list = _pageindexqlrepository.GetByWhere("Status=@1 and Lang_ID=@2 Order By Sort", new object[] { true, langid });
                foreach (var l in list)
                {
                    sb.Append("<option value='" + l.ID.ToString() + "'>" + l.Name + "</option>");
                }
            }
            else if (modelid == "2")
            {
                if (hasnewitem) { sb.Append("<option value='-1'>新建單元</option>"); }
                var list = _messageqlrepository.GetByWhere("Status=@1 and Lang_ID=@2 Order By Sort", new object[] { true, langid });
                foreach (var l in list)
                {
                    sb.Append("<option value='" + l.ID.ToString() + "'>" + l.Name + "</option>");
                }
            }
            else if (modelid == "3")
            {
                if (hasnewitem) { sb.Append("<option value='-1'>新建單元</option>"); }
                var list = _activesqlrepository.GetByWhere("Status=@1 and Lang_ID=@2 Order By Sort", new object[] { true, langid });
                foreach (var l in list)
                {
                    sb.Append("<option value='" + l.ID.ToString() + "'>" + l.Name + "</option>");
                }
            }
            else if (modelid == "4")
            {
                if (hasnewitem) { sb.Append("<option value='-1'>新建單元</option>"); }
                var list = _filesqlrepository.GetByWhere("Status=@1 and Lang_ID=@2 Order By Sort", new object[] { true, langid });
                foreach (var l in list)
                {
                    sb.Append("<option value='" + l.ID.ToString() + "'>" + l.Name + "</option>");
                }
            }
            else if (modelid == "5")
            {
                if (hasnewitem) { sb.Append("<option value='-1'>新建單元</option>"); }
                var list = _mapsqlrepository.GetByWhere("Status=@1 and Lang_ID=@2 Order By Sort", new object[] { true, langid });
                foreach (var l in list)
                {
                    sb.Append("<option value='" + l.ID.ToString() + "'>" + l.Name + "</option>");
                }
            }
            else if (modelid == "7")
            {
                if (hasnewitem) { sb.Append("<option value='-1'>新建單元</option>"); }
                var list = _articlesqlrepository.GetByWhere("Status=@1 and Lang_ID=@2 Order By Sort", new object[] { true, langid });
                foreach (var l in list)
                {
                    sb.Append("<option value='" + l.ID.ToString() + "'>" + l.Name + "</option>");
                }
            }
            else if (modelid == "11")
            {
                if (hasnewitem) { sb.Append("<option value='-1'>新建單元</option>"); }
                var list = _formsqlrepository.GetByWhere("Lang_ID=@2 and IsVerift=1 Order By Sort", new object[] { true, langid });
                foreach (var l in list)
                {
                    sb.Append("<option value='" + l.ID.ToString() + "'>" + l.Name + "</option>");
                }
            }

            return sb.ToString();
        } 
        #endregion

        #region GetModel
        public MenuEditModel GetModel(string id)
        {
            var model = new MenuEditModel();
            var data = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
            if (data.Count() > 0)
            {
                _sqlrepository.Mapping<Menu, MenuEditModel>(data.First(), model);
            }
            model.ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/MenuImage/" + model.ImgNameOri);
            model.DeleteUploadFile = "N";
            return model;
        }
        #endregion

        #region Create
        public string Create(MenuEditModel model, string account, string username)
        {

            //1.create message
            var datetime = DateTime.Now;
            if (model.LinkMode == 2 && model.ModelItemID==-1) {
                int newid = 0;
                if (model.ModelID == 1) {
                    _IPageEditManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 2)
                {
                    _IMessageManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 3)
                {
                    _IModelActiveManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 4)
                {
                    _IModelFileDownloadManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 6)
                {
                    _ILangManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 7) { _IModelArticleManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid); }
                else if (model.ModelID == 8)
                {
                   // _IKnowledgeManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 9){}
                else if (model.ModelID ==10){}
                else if (model.ModelID == 11){_IModelFormManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid); }
                model.ModelItemID = newid;
            }

            var menucnt = _sqlrepository.GetCountUseWhere("LangID=@1 and ParentID=@2", new object[] { model.LangID, model.ParentID });
            var sort = menucnt + 1;
            var menu = new Menu()
            {
                ImageHeight = model.ImageHeight,
                ImgNameOri = model.ImgNameOri,
                ImgNameThumb = model.ImgNameThumb,
                ImgShowName = model.ImgShowName,
                LangID = model.LangID,
                LinkMode = model.LinkMode,
                MenuLevel = model.MenuLevel,
                MenuName = model.MenuName,
                ModelID = model.ModelID,
                ModelItemID = model.ModelItemID,
                OpenMode = model.OpenMode,
                ShowMode = model.ShowMode,
                Sort = sort,
                Status = true,
                UpdateDatetime = datetime,
                UpdateUser = account,
                WindowHeight = model.WindowHeight,
                WindowWidth = model.WindowWidth,
                MenuType = model.MenuType,
                ParentID = model.ParentID,
                LinkUrl = model.LinkUrl ,
                LinkUploadFileName=model.LinkUploadFileName,
                LinkUploadFilePath=model.LinkUploadFilePath,
                 DisplayName = model.DisplayName,
                 ICon= model.ICon == null ? "" : model.ICon
            };

            var r = _sqlrepository.Create(menu);
            if (r > 0)
            {
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }

        }
        #endregion

        #region Update
        public string Update(MenuEditModel model, string account, string username)
        {
            var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { model.ID });
            var oldfilename = olddata.First().ImgNameThumb;
            var oldfileoriname = olddata.First().ImgNameOri;
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
                "\\UploadImage\\MenuImage\\" + oldfilename;
            var oldoriroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath +
              "\\UploadImage\\MenuImage\\" + oldfileoriname;
            if (model.ImgNameOri.IsNullorEmpty())
            {
                model.ImgNameThumb = "";
                model.ImgNameOri = "";
                model.ImgShowName = "";
            }
            var r = 0;
            if (model.DeleteUploadFile == "Y" && olddata.First().LinkUploadFilePath.IsNullorEmpty()==false)
            {
                if (System.IO.File.Exists(olddata.First().LinkUploadFilePath)) {
                    System.IO.File.Delete(olddata.First().LinkUploadFilePath);
                }
            }
            if (model.LinkMode != 4)
            {
                if (System.IO.File.Exists(olddata.First().LinkUploadFilePath))
                {
                    System.IO.File.Delete(olddata.First().LinkUploadFilePath);
                }
                model.LinkUploadFileName = "";
                model.LinkUploadFilePath = "";
            }
            if (model.LinkMode != 3)
            {
                model.LinkUrl = "";
            }
            //1.create message
            var datetime = DateTime.Now;
            if (model.LinkMode == 2 && model.ModelItemID == -1)
            {
                int newid = 0;
                if (model.ModelID == 1)
                {
                    _IPageEditManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 2)
                {
                    _IMessageManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 3)
                {
                    _IModelActiveManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 4)
                {
                    _IModelFileDownloadManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 6)
                {
                    _ILangManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 7) { _IModelArticleManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid); }
                else if (model.ModelID == 8)
                {
                    // _IKnowledgeManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid);
                }
                else if (model.ModelID == 9) { }
                else if (model.ModelID == 10) { }
                else if (model.ModelID == 11) { _IModelFormManager.AddUnit(model.MenuName, model.LangID.ToString(), account, ref newid); }
                model.ModelItemID = newid;
            }
        
            var ad = new Menu()
            {
                ImageHeight = model.ImageHeight,
                ImgNameOri = model.ImgNameOri,
                ImgNameThumb = model.ImgNameThumb,
                ImgShowName = model.ImgShowName,
                LangID = model.LangID,
                LinkMode = model.LinkMode,
                MenuLevel = model.MenuLevel,
                MenuName = model.MenuName,
                ModelID = model.ModelID,
                ModelItemID = model.ModelItemID,
                OpenMode = model.OpenMode,
                ShowMode = model.ShowMode,
                UpdateDatetime = datetime,
                UpdateUser = account,
                WindowHeight = model.WindowHeight,
                WindowWidth = model.WindowWidth,
                MenuType = model.MenuType,
                 ID=model.ID,
                LinkUrl = model.LinkUrl==null?"": model.LinkUrl,
                LinkUploadFileName = model.LinkUploadFileName,
                LinkUploadFilePath = model.LinkUploadFilePath,
                DisplayName = model.DisplayName==null?"": model.DisplayName,
                ICon = model.ICon==null?"": model.ICon
            };

             r = _sqlrepository.Update(ad);
            if (r > 0)
            {
                if (model.ImgNameOri.IsNullorEmpty() || model.ImgNameOri != oldfileoriname)
                {
                    if (System.IO.File.Exists(oldroot))
                    {
                        System.IO.File.Delete(oldroot);
                    }
                    if (System.IO.File.Exists(oldoriroot))
                    {
                        System.IO.File.Delete(oldoriroot);
                    }
                }
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }

        }

        #endregion

        #region GetMenu
        public List<Menu> GetMenu(string languageID,string menutype)
        {
            if (menutype != "" && languageID != "")
            {
                return _sqlrepository.GetByWhere("LangID=@1 and MenuType=@2 Order by Sort", new object[] { languageID, menutype }).ToList();
            }
            else if (languageID!="" && menutype == "")
            {
                return _sqlrepository.GetByWhere("LangID=@1 and MenuType=@2 Order by Sort", new object[] { languageID, menutype }).ToList();
            }
            else {
                return _sqlrepository.GetAll().ToList();
            }
        }
        #endregion

        #region DeleteMenu
        public string DeleteMenu(string menuid)
        {
            var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { menuid });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\MenuImage\\";
            var r = _sqlrepository.Delete(olddata.First());
            if (r > 0) {
                if (System.IO.File.Exists(oldroot + "\\" + olddata.First().ImgNameOri))
                {
                    System.IO.File.Delete(oldroot + "\\" + olddata.First().ImgNameOri);
                }
                if (System.IO.File.Exists(oldroot + "\\" + olddata.First().ImgNameThumb))
                {
                    System.IO.File.Delete(oldroot + "\\" + olddata.First().ImgNameThumb);
                }
            }
             
            if (olddata.First().MenuLevel == 1)
            {
                var oldlevel2 = _sqlrepository.GetByWhere("ParentID=@1", new object[] { menuid });
                var sortl1 =_sqlrepository.GetByWhere("MenuLevel=@1 order by sort", new object[] { 1 });
                var sortidx = 1;
                foreach (var l1 in sortl1) {
                    _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { sortidx, l1.ID });
                    sortidx += 1;
                }
                foreach (var l2 in oldlevel2)
                {
                    r = _sqlrepository.Delete(l2);
                    if (r > 0)
                    {
                        if (System.IO.File.Exists(oldroot + "\\" + l2.ImgNameOri))
                        {
                            System.IO.File.Delete(oldroot + "\\" + l2.ImgNameOri);
                        }
                        if (System.IO.File.Exists(oldroot + "\\" + l2.ImgNameThumb))
                        {
                            System.IO.File.Delete(oldroot + "\\" + l2.ImgNameThumb);
                        }
                        var oldlevel3 = _sqlrepository.GetByWhere("ParentID=@1", new object[] { l2.ID });
                        foreach (var l3 in oldlevel3)
                        {
                            r = _sqlrepository.Delete(l3);
                            if (r > 0)
                            {
                                if (System.IO.File.Exists(oldroot + "\\" + l3.ImgNameOri))
                                {
                                    System.IO.File.Delete(oldroot + "\\" + l3.ImgNameOri);
                                }
                                if (System.IO.File.Exists(oldroot + "\\" + l3.ImgNameThumb))
                                {
                                    System.IO.File.Delete(oldroot + "\\" + l3.ImgNameThumb);
                                }
                            }
                        }
                    }
                }

            }
            else if (olddata.First().MenuLevel == 2)
            {
                var sortl2 = _sqlrepository.GetByWhere("MenuLevel=@1 and  ParentID=@2 order by sort", new object[] { 2, olddata.First().ParentID });
                var sortidx = 1;
                foreach (var l2 in sortl2)
                {
                    _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { sortidx, l2.ID });
                    sortidx += 1;
                }
                var oldlevel3= _sqlrepository.GetByWhere("ParentID=@1", new object[] { menuid });
                foreach (var l3 in oldlevel3) {
                     r=_sqlrepository.Delete(l3);
                    if (r > 0)
                    {
                        if (System.IO.File.Exists(oldroot + "\\" + l3.ImgNameOri))
                        {
                            System.IO.File.Delete(oldroot + "\\" + l3.ImgNameOri);
                        }
                        if (System.IO.File.Exists(oldroot + "\\" + l3.ImgNameThumb))
                        {
                            System.IO.File.Delete(oldroot + "\\" + l3.ImgNameThumb);
                        }
                    }
                }
            }
            return "刪除成功";
        }
        #endregion

        #region Menudisabled
        public string Menudisabled(string menuid)
        {
            var r = _sqlrepository.Update("Status=@1", "ID=@2", new object[] { false, menuid });
            if (r > 0) {return  "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region Menueabled
        public string Menueabled(string menuid)
        {
            var r = _sqlrepository.Update("Status=@1", "ID=@2", new object[] { true, menuid });
            if (r > 0) { return "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region UpdateSort
        public string UpdateSort(int menuid, string type, string account, string username)
        {
            try
            {
                var oldmodel = new List<Menu>();
                var olddata = _sqlrepository.GetByWhere("ID=@1", new object[] { menuid });
                oldmodel = _sqlrepository.GetByWhere("LangID=@1 and ParentID=@2 and MenuType=@3", new object[] { olddata.First().LangID, olddata.First().ParentID, olddata.First().MenuType }).OrderBy(v => v.Sort).ToList();
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var index = oldmodel.FindIndex(v => v.ID == menuid);
                var r = 0;
                if (type == "next")
                {
                    if (index + 1 >= oldmodel.Count()) { return "更新成功"; }
                    r= _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { oldmodel[index + 1].Sort, oldmodel[index].ID });
                    r=_sqlrepository.Update("Sort=@1", "ID=@2", new object[] { oldmodel[index].Sort, oldmodel[index+1].ID });
                }
                else {
                    if (index - 1 < 0) { return "更新成功"; }
                    r = _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { oldmodel[index -1].Sort, oldmodel[index].ID });
                    r = _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { oldmodel[index].Sort, oldmodel[index -1].ID });
                }
              
                if (r > 0)
                {
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("更新排序失敗:" + " error:" + ex.Message);
                return "更新排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region GetMenuOption
        public string GetMenuOption(int langid, int level, int parentid, int type, int modelid = 0)
        {
            List<Menu> data = new List<Menu>();
            data = _sqlrepository.GetByWhere("LangID=@1 and MenuType=@2 and Status=1", new object[] { langid,  type }).ToList();
            if (modelid == 0)
            {
                var l3model = data.Where(v => v.MenuLevel == 3 && v.LinkMode == 2 && (v.ModelID == 2 || v.ModelID == 3 || v.ModelID == 18 || v.ModelID == 19));
                var l3partnt = data.Where(v => l3model.Any(X => X.ParentID == v.ID));
                var l3l1partnt = data.Where(v => l3partnt.Any(X => X.ParentID == v.ID));
                var l2model = data.Where(v => v.MenuLevel == 2 && v.LinkMode == 2 && (v.ModelID == 2 || v.ModelID == 3 || v.ModelID == 18 || v.ModelID == 19));
                var l2partnt = data.Where(v => l2model.Any(X => X.ParentID == v.ID));
                var l1model = data.Where(v => v.MenuLevel == 1 && v.LinkMode == 2 && (v.ModelID == 2 || v.ModelID == 3 || v.ModelID == 18 || v.ModelID == 19));
                data = l3model.Union(l3partnt).Union(l3l1partnt).Union(l2model).Union(l2partnt).Union(l1model).ToList();
            }
            else {
                var l3model = data.Where(v => v.MenuLevel == 3 && v.LinkMode == 2 && (v.ModelID == modelid));
                var l3partnt = data.Where(v => l3model.Any(X => X.ParentID == v.ID));
                var l3l1partnt = data.Where(v => l3partnt.Any(X => X.ParentID == v.ID));
                var l2model = data.Where(v => v.MenuLevel == 2 && v.LinkMode == 2 && (v.ModelID == modelid));
                var l2partnt = data.Where(v => l2model.Any(X => X.ParentID == v.ID));
                var l1model = data.Where(v => v.MenuLevel == 1 && v.LinkMode == 2 && (v.ModelID == modelid));
                data = l3model.Union(l3partnt).Union(l3l1partnt).Union(l2model).Union(l2partnt).Union(l1model).ToList();
            }
       
            if (level == 1)
            {
                data = data.Where(v => v.MenuLevel == 1).OrderBy(v => v.Sort).ToList();
            }
            else
            {
                data = data.Where(v => v.ParentID == parentid).OrderBy(v => v.Sort).ToList();
            }
            var sb = new StringBuilder("<option value=''>無</option>");

            foreach (var item in data)
            {
                sb.Append("<option value='" + item.ID + "'>" + item.MenuName + "</option>");
            }
            return sb.ToString();

        } 
        #endregion

        #region GetMenuUrl
        public string[] GetMenuUrl(string id)
        {
            if (id == null) {return new string[] { "", "" };}
            var menu = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
            if (menu.Count() == 0) { return new string[] { "",""}; }
            var _munu = menu.First();
            if (_munu.LinkMode == 2)
            {
                if (_munu.ModelID == 1) {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "PageEdit"),"id", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 2)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Message"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 3)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Active"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 4)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "FileDownload"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID ==5)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("WebSiteEdit", "WebsiteMap"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 6)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Lang"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 7)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Article"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 8)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelColumnSetting", "Knowledge"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 9)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Forum"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 10)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Industry"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 11)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("MailModelItem", "Form"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 12)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Course"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 13)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Product"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 14)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("CartForm", "Cart"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 15)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("GroupSetting", "BaseData"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 16)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "FAQ"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 17)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "EventList"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 18)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Video"), "mainid", _munu.ModelItemID.ToString() };
                }
                else if (_munu.ModelID == 19)
                {
                    HttpContext.Current.Session["IsFromClick"] = null;
                    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    return new string[] { _munu.LinkMode.ToString(), helper.Action("ModelItem", "Patent"), "mainid", _munu.ModelItemID.ToString() };
                }
            }
            else if (_munu.LinkMode ==3)
            {
                HttpContext.Current.Session["IsFromClick"] = null;
                UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                return new string[] { "2", helper.Action("MenuLinkEdit", "Menu"), "menuid", _munu.ID.ToString() };


                //if (_munu.LinkUrl.IndexOf("~") == 0)
                //{
                //    UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                //    string root = helper.Content(_munu.LinkUrl);
                //    return new string[] { _munu.LinkMode.ToString(), root };
                //}
                //else {
                //    return new string[] { _munu.LinkMode.ToString(), _munu.LinkUrl };
                //}

                //var menuurl = _urlsqlrepository.GetAll();
                //if (menuurl != null && menuurl.Count() > 0) {
                //    var menudist = menuurl.ToDictionary(v => v.MenuName, v => v.MenuPath);
                //    if (menudist.ContainsKey(_munu.MenuName)) {
                //        return new string[] { _munu.LinkMode.ToString(), menudist[_munu.MenuName] };
                //    }
                //}
                //  return new string[] { "", "" };
            }
              return new string[] { "", "" };
        }
        #endregion

        #region GetMenuIDList
        public Menu[] GetMenuIDList(string menuid)
        {
            var model = new MenuEditModel();
            var data = _sqlrepository.GetByWhere("ParentID=@1 and Status=1 Order By Sort", new object[] { menuid }).ToArray();
         
            return data;
        }
        #endregion

        #region GetModelItemList
        public string GetModelItemList(string menuid, string langid)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<option value=''>請選擇</option>");
            var menu = _sqlrepository.GetByWhere("ID=@1", new object[] { menuid });
            var modelid = menu.First().ModelID.ToString();
            var modelitemid = menu.First().ModelItemID.ToString();
            if (modelid == "2")
            {
                var list = _IMessageManager.PagingItem(modelitemid, new MessageSearchModel()
                {
                    Limit = -1,
                    Sort="Sort"
                });
                foreach (var l in list.rows)
                {sb.Append("<option value='" + l.ItemID.ToString() + "'>" + l.Title + "</option>"); }
            }
            else if (modelid == "3")
            {
                var list = _IModelActiveManager.PagingItem(modelitemid, new  ActiveSearchModel()
                {
                    Limit = -1,
                    Sort = "Sort"
                });
                foreach (var l in list.rows)
                {sb.Append("<option value='" + l.ItemID.ToString() + "'>" + l.Title + "</option>"); }
            }
            return sb.ToString();
        }

        #endregion


        public string UpdateMenuLink(string linkurl, string menuid, string account, string userName)
        {
            var r = _sqlrepository.Update("LinkUrl=@1", "ID=@2", new object[] { linkurl, menuid });
            if (r > 0) { return "設定完成"; } else { return "設定失敗"; }
        }
    }
}
