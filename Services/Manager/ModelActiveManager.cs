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
using System.ServiceModel.Syndication;
using System.Web;
using ViewModels.DBModels;
using System.Configuration;

namespace Services.Manager
{
    public class ModelActiveManager : IModelActiveManager
    {
        readonly SQLRepository<ModelActiveEditMain> _sqlrepository;
        readonly SQLRepository<ActiveItem> _activeitemsqlrepository;
        readonly SQLRepository<GroupActive> _groupsqlrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<ActiveUnitSetting> _unitsqlitemrepository;
        readonly SQLRepository<ColumnSetting> _columnsqlrepository;
        readonly SQLRepository<ActivePhoto> _photosqlrepository;
        readonly SQLRepository<ActivePhotoCount> _activephotocountsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<PageLayout> _pagesqlrepository;
        readonly SQLRepository<ClickCountTable> _clicktablesqlitemrepository;
        readonly SQLRepository<ActiveDateRange> _ActiveDateRangelitemrepository;
        readonly SQLRepository<ActiveFiles> _ActiveFilessqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        readonly SQLRepository<SiteConfig> _siteconfigqlrepository;
        public ModelActiveManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelActiveEditMain;
            _seosqlrepository = sqlinstance.SEO;
            _activeitemsqlrepository = sqlinstance.ActiveItem;
            _groupsqlrepository = sqlinstance.GroupActive;
            _unitsqlitemrepository = sqlinstance.ActiveUnitSetting;
            _columnsqlrepository = sqlinstance.ColumnSetting;
            _photosqlrepository = sqlinstance.ActivePhoto;
            _menusqlrepository = sqlinstance.Menu;
            _pagesqlrepository = sqlinstance.PageLayout;
            _clicktablesqlitemrepository = sqlinstance.ClickCountTable;
            _activephotocountsqlrepository = sqlinstance.ActivePhotoCount;
            _ActiveDateRangelitemrepository = sqlinstance.ActiveDateRange;
            _ActiveFilessqlrepository = sqlinstance.ActiveFiles;
            _Usersqlrepository = sqlinstance.Users;
            _siteconfigqlrepository = sqlinstance.SiteConfig;
        }

        #region GetAll
        public IEnumerable<ModelActiveEditMain> GetAll()
        {
            return _sqlrepository.GetAll();
        }

        #endregion

        #region Paging
        public Paging<ModelActiveEditMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelActiveEditMain>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Lang_ID=@1");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @2");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            foreach (var r in Paging.rows)
            {
                r.EID = StringEncrypt.encrypt(r.ID.ToString());
                var dd = StringEncrypt.decrypt(r.EID);
            }
            return Paging;
        }
        #endregion

        #region Where
        public IEnumerable<ModelActiveEditMain> Where(ModelActiveEditMain model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region AddUnit
        public string AddUnit(string name, string langid, string account, ref int newid)
        {
            var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 Order By Sort", new object[] { langid }).OrderBy(v => v.Sort);
            var idx = 2;
            foreach (var mdata in alldata)
            {
                mdata.Sort = idx;
                _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
                idx += 1;
            }
            var Model = new ModelActiveEditMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 3;
            Model.Name = name;
            Model.CreateDate = DateTime.Now;
            Model.UpdateDate = DateTime.Now;
            Model.CreateUser = account;
            Model.UpdateUser = account;
            Model.Sort = 1;
            Model.Status = true;
            var r = _sqlrepository.Create(Model);
            newid = Model.ID;
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

        #region UpdateUnit
        public string UpdateUnit(string name, string id, string account)
        {
            var r = _sqlrepository.Update("Name=@1", "ID=@2", new object[] { name, id });
            if (r > 0) { return "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string langid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ModelActiveEditMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1  and Lang_ID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelActiveEditMain> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1   and Lang_ID=@2", new object[] { qseq, langid }).OrderBy(v => v.Sort).ToArray();
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _sqlrepository.Update(tempmodel);
                    }
                }

                oldmodel.First().Sort = seq;
                r = _sqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新訊息管理排序失敗:" + " error:" + ex.Message);
                return "更新訊息管理排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var m = _menusqlrepository.GetByWhere("ModelID=3 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                    var p = _pagesqlrepository.GetByWhere("ModelID=3 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (p.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelActiveEditMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                 
                  
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除訊息管理單元失敗:ID=" + idlist[idx]);
                    }
                    else {
                        _ActiveDateRangelitemrepository.DelDataUseWhere("ModelID=@1", new object[] { idlist[idx] });
                        var allitems = _activeitemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        _seosqlrepository.DelDataUseWhere("TypeName='ActiveItem' and TypeID=@1", new object[] { idlist[idx] });
                        _unitsqlitemrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });
                        var olditem = _activeitemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ActiveItem\\";
                        foreach (var items in olditem)
                        {
                            _activeitemsqlrepository.Delete(items);
                            _seosqlrepository.DelDataUseWhere("TypeName='ActiveItem' and TypeID=@1", new object[] { items.ItemID });
                            if (items.UploadFileName.IsNullorEmpty() == false)
                            {
                                if (System.IO.File.Exists(items.UploadFilePath))
                                {
                                    System.IO.File.Delete(items.UploadFilePath);
                                }
                            }
                            if (items.ImageFileName.IsNullorEmpty() == false)
                            {

                                if (System.IO.File.Exists(oldroot + "\\" + items.ImageFileName))
                                {
                                    System.IO.File.Delete(oldroot + "\\" + items.ImageFileName);
                                }
                            }
                            if (items.RelateImageFileName.IsNullorEmpty() == false)
                            {

                                if (System.IO.File.Exists(oldroot + "\\" + items.RelateImageFileName))
                                {
                                    System.IO.File.Delete(oldroot + "\\" + items.RelateImageFileName);
                                }
                            }

                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                    NLogManagement.SystemLogInfo("刪除訊息管理單元失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _sqlrepository.GetByWhere("Lang_ID=@1", new object[] { langid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _sqlrepository.Update(tempmodel);
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除訊息管理單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region GetActiveSelectList
        public IList<SelectListItem> GetActiveSelectList(string lang_id)
        {
            var list = _sqlrepository.GetByWhere("Status=@1 and Lang_ID=@2 Order By Sort", new object[] { true, lang_id });
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            item.Add(new System.Web.Mvc.SelectListItem() { Text = "未設定", Value = "" });
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Name, Value = l.ID.ToString() });
            }
            return item;
        }
        #endregion

        #region GetActiveItem
        public string GetActiveItem(string modelid)
        {
            var list = _activeitemsqlrepository.GetByWhere("Enabled=@1 and ModelID=@2 Order By Sort", new object[] { true, modelid });
            var sd = new System.Text.StringBuilder();
            foreach (var l in list)
            {
                sd.Append("<option value='" + l.ItemID + "'>" + l.Title + "</option>");
            }
            return sd.ToString();
        }
        #endregion

        #region GetSEO
        public SEOViewModel GetSEO(string mainid)
        {
            var seodata = _seosqlrepository.GetByWhere("TypeName=@1 and TypeID=@2", new object[] { "Active", mainid });
            var model = new SEOViewModel();
            if (seodata.Count() > 0)
            {
                model.ID = seodata.First().ID;
                model.TypeID = seodata.First().TypeID.ToString();
                model.Description = seodata.First().Description;
                model.WebsiteTitle = seodata.First().Title;
                model.Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10};
            }
            else
            {
                model.Keywords = new string[10];
                model.TypeID = mainid;
            }
            return model;
        }
        #endregion

        #region SaveSEO
        public string SaveSEO(SEOViewModel model, string LangID)
        {
            var seomodel = new SEO()
            {
                Description = model.Description == null ? "" : model.Description,
                Keywords1 = model.Keywords[0],
                Keywords2 = model.Keywords[1],
                Keywords3 = model.Keywords[2],
                Keywords4 = model.Keywords[3],
                Keywords5 = model.Keywords[4],
                Keywords6 = model.Keywords[5],
                Keywords7 = model.Keywords[6],
                Keywords8 = model.Keywords[7],
                Keywords9 = model.Keywords[8],
                Keywords10 = model.Keywords[9],
                Title = model.WebsiteTitle == null ? "" : model.WebsiteTitle,
                TypeName = "Active",
                TypeID = int.Parse(model.TypeID),
                Lang_ID = int.Parse(LangID)
            };
            var r = 0;
            if (model.ID == -1)
            {
                r = _seosqlrepository.Create(seomodel);
            }
            else
            {
                seomodel.ID = model.ID;
                r = _seosqlrepository.Update(seomodel);
            }
            if (r > 0)
            {
                return "儲存成功";
            }
            else
            {
                return "儲存失敗";
            }
        }
        #endregion

        #region GetGroupSelectList
        public IList<SelectListItem> GetGroupSelectList(string mainid)
        {
            var list = _groupsqlrepository.GetByWhere("Enabled=@1 and Main_ID=@2 Order By Sort", new object[] { true, mainid });
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            item.Add(new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Group_Name, Value = l.ID.ToString() });
            }
            return item;
        }
        #endregion

        #region PagingGroup
        public Paging<GroupActive> PagingGroup(SearchModelBase model)
        {
            var Paging = new Paging<GroupActive>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Main_ID=" + model.ModelID);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Group_Name like @1");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _groupsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);

            Paging.total = _groupsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region EditGroup
        public string EditGroup(string name, string id, string mainid, string account)
        {
            var r = 0;
            if (id == "-1" || id.IsNullorEmpty())
            {
                var alldata = _groupsqlrepository.GetByWhere("Main_ID=@1", new object[] { mainid });
                if (alldata.Any(v => v.Group_Name == name))
                {
                    return "類別名稱已經存在";
                }
                var maxsort = _groupsqlrepository.GetDataCaculate("Max(Sort)", "Main_ID=@1", new object[] { mainid });
                var group = new GroupActive();
                group.Group_Name = name;
                group.Enabled = true;
                group.Readonly = false;
                group.Seo_Manage = false;
                group.Main_ID = int.Parse(mainid);
                group.Sort = maxsort + 1;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                r = _groupsqlrepository.Create(group);
                if (r > 0)
                {
                    return "新增成功";
                }
                else
                {
                    return "新增失敗";
                }
            }
            else
            {
                var checkdata = _groupsqlrepository.GetByWhere("Group_Name=@1 and ID!=@2  AND Main_ID=@3", new object[] { name, id, mainid });
                if (checkdata.Count() > 0)
                {
                    return "類別名稱已經存在";
                }

                var group = new GroupActive();
                group.ID = int.Parse(id);
                group.Group_Name = name;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                r = _groupsqlrepository.Update(group);
                if (r > 0)
                {
                    return "修改成功";
                }
                else
                {
                    return "修改失敗";
                }

            }
        }
        #endregion

        #region DeleteGroup
        public string DeleteGroup(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var mainid = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var users = _activeitemsqlrepository.GetByWhere("GroupID=@1", new object[] { idlist[idx] });
                    mainid = _groupsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().Main_ID.Value;
                    if (users.Count() > 0)
                    {
                        var gname = _groupsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().Group_Name;
              
                        return "群組名稱:" + gname + " 目前已被使用,無法刪除";
                    }
                    var entity = new GroupActive();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _groupsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除群組失敗:ID=" + idlist[idx]);
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除群組:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _groupsqlrepository.GetByWhere("Main_ID=@1", new object[] { mainid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _groupsqlrepository.Update(tempmodel);
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region UpdateGroupStatus
        public string UpdateGroupStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new GroupActive();
                entity.Enabled = status ? true : false;
                entity.UpdateDatetime = DateTime.Now;
                entity.ID = int.Parse(id);
                entity.UpdateUser = account;
                var r = _groupsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改GroupActive顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改GroupActive顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateGroupSeq
        public string UpdateGroupSeq(int id, int seq, string mainid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _groupsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<GroupActive> mtseqdata = null;
                    mtseqdata = _groupsqlrepository.GetByWhere("Sort>@1 and Main_ID=@2", new object[] { seq, mainid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _groupsqlrepository.GetCountUseWhere("Main_ID=@1", new object[] { mainid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<GroupActive> ltseqdata = null;
                    ltseqdata = _groupsqlrepository.GetByWhere("Sort<=@1  and Main_ID=@2", new object[] { qseq, mainid }).OrderBy(v => v.Sort).ToArray();
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _groupsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _groupsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新訊息管理排序失敗:" + " error:" + ex.Message);
                return "更新訊息管理排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        //Unitsetting
        #region GetUnitModel
        public ActiveUnitSettingModel GetUnitModel(string mainid)
        {
            var data = _unitsqlitemrepository.GetByWhere("MainID=@1", new object[] { mainid });
            var model = new ActiveUnitSettingModel();
            model.MainID = int.Parse(mainid);
            if (data.Count() > 0)
            {
                model = new ActiveUnitSettingModel()
                {
                    ClassOverview = data.First().ClassOverview,
                    Column1 = data.First().Column1,
                    Column2 = data.First().Column2,
                    Column3 = data.First().Column3,
                    Column4 = data.First().Column4,
                    Column5 = data.First().Column5,
                    Column6 = data.First().Column6,
                    Column7 = data.First().Column7,
                    Column8 = data.First().Column8,
                    Column9 = data.First().Column9,
                    Column10 = data.First().Column10,
                    Column11 = data.First().Column11,
                    Column12 = data.First().Column12,
                    Column13 = data.First().Column13,
                    Column14 = data.First().Column14,
                    Column15 = data.First().Column15,
                    Column16 = data.First().Column16,
                    Column17 = data.First().Column17,
                    IsForward = data.First().IsForward,
                    IsPrint = data.First().IsPrint,
                    IsRSS = data.First().IsRSS,
                    IsShare = data.First().IsShare,
                    MemberAuth = data.First().MemberAuth,
                    MainID = data.First().MainID,
                    ShowCount = data.First().ShowCount,
                    ID = data.First().ID,
                    EMailAuth = data.First().EMailAuth,
                    VIPAuth = data.First().VIPAuth,
                    EnterpriceStudentAuth = data.First().EnterpriceStudentAuth,
                    GeneralStudentAuth = data.First().GeneralStudentAuth
                };
                var cs = data.First().ColumnSetting;
                if (cs.IsNullorEmpty() == false)
                {
                    var csarr = cs.Split('@');
                    var cname = csarr[0].Split(',');
                    var cuse = csarr[1].Split(',');
                    for (var v = 0; v < cname.Length; v++)
                    {
                        model.UnitSettingColumnList.Add(new UnitSettingColumn()
                        {
                            Name = cname[v],
                            Sellected = int.Parse(cuse[v])
                        });
                    }
                }
            }
            if (model.UnitSettingColumnList.Count() == 0)
            {
                var columnlist = _columnsqlrepository.GetByWhere("Type='Active'", null).OrderBy(v => v.Sort);
                foreach (var c in columnlist)
                {
                    model.UnitSettingColumnList.Add(new UnitSettingColumn()
                    {
                        Name = c.ColumnName,
                        Sellected = 1
                    });
                }
            }
            model.ColumnNameMapping = new Dictionary<string, string>();
            model.ColumnNameMapping.Add("發佈日期", model.Column1.IsNullorEmpty() ? "發佈日期" : model.Column1);
            model.ColumnNameMapping.Add("標題", model.Column2.IsNullorEmpty() ? "標題" : model.Column2);
            model.ColumnNameMapping.Add("類別", model.Column3.IsNullorEmpty() ? "類別" : model.Column3);
            model.ColumnNameMapping.Add("簡介", model.Column4.IsNullorEmpty() ? "簡介" : model.Column4);
            return model;
        }
        #endregion

        #region ColumnPaging
        public Paging<ColumnSetting> ColumnPaging(SearchModelBase model)
        {
            var Paging = new Paging<ColumnSetting>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Type='Active'");
            whereobj.Add(model.LangId);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Name like @2");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _columnsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _columnsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region UpdateColumnStatus
        public string UpdateColumnStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new ColumnSetting();
                entity.Show = status ? true : false;
                entity.ID = int.Parse(id);
                var r = _columnsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改ColumnSetting顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改ColumnSetting顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateColumnSeq
        public string UpdateColumnSeq(int id, int seq, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _columnsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ColumnSetting> mtseqdata = null;
                    mtseqdata = _columnsqlrepository.GetByWhere("Sort>@1 and Type=@2", new object[] { seq, "Active" }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _columnsqlrepository.GetCountUseWhere("Type=@1", new object[] { "Active" });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ColumnSetting> ltseqdata = null;
                    ltseqdata = _columnsqlrepository.GetByWhere("Sort<=@1  and Type=@2", new object[] { qseq, "Active" }).OrderBy(v => v.Sort).ToArray();
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _columnsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _columnsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新ColumnSetting排序失敗:" + " error:" + ex.Message);
                return "更新ColumnSetting排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetUnitModel
        public string SetUnitModel(ActiveUnitSettingModel model, string account)
        {
            var newmodel = new ActiveUnitSetting();
            newmodel.UpdateDatetime = DateTime.Now;
            newmodel.UpdateUser = account;
            var r = 0;
            var columnsetting = "";
            if (model.ColumnName != null)
            {
                columnsetting = string.Join(",", model.ColumnName) + "@" + string.Join(",", model.ColumnUse);
            }
            if (model.ID == -1)
            {
                newmodel.MainID = model.MainID.Value;
                newmodel.ClassOverview = model.ClassOverview;
                newmodel.Column1 = model.Column1;
                newmodel.Column2 = model.Column2;
                newmodel.Column3 = model.Column3;
                newmodel.Column4 = model.Column4;
                newmodel.Column5 = model.Column5;
                newmodel.Column6 = model.Column6;
                newmodel.Column7 = model.Column7;
                newmodel.Column8 = model.Column8;
                newmodel.Column9 = model.Column9;
                newmodel.Column10 = model.Column10;
                newmodel.Column11 = model.Column11;
                newmodel.Column12 = model.Column12;
                newmodel.Column13 = model.Column13;
                newmodel.Column14 = model.Column14;
                newmodel.Column15 = model.Column15;
                newmodel.Column16 = model.Column16;
                newmodel.Column17 = model.Column17;
                newmodel.IsRSS = model.IsRSS;
                newmodel.IsShare = model.IsShare;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsForward = model.IsForward;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.ShowCount = model.ShowCount;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                newmodel.ColumnSetting = columnsetting;
                r = _unitsqlitemrepository.Create(newmodel);
            }
            else
            {
                newmodel.ID = model.ID.Value;
                newmodel.MainID = model.MainID.Value;
                newmodel.ClassOverview = model.ClassOverview;
                newmodel.Column1 = model.Column1 == null ? "" : model.Column1;
                newmodel.Column2 = model.Column2 == null ? "" : model.Column2;
                newmodel.Column3 = model.Column3 == null ? "" : model.Column3;
                newmodel.Column4 = model.Column4 == null ? "" : model.Column4;
                newmodel.Column5 = model.Column5 == null ? "" : model.Column5;
                newmodel.Column6 = model.Column6 == null ? "" : model.Column6;
                newmodel.Column7 = model.Column7 == null ? "" : model.Column7;
                newmodel.Column8 = model.Column8 == null ? "" : model.Column8;
                newmodel.Column9 = model.Column9 == null ? "" : model.Column9;
                newmodel.Column10 = model.Column10 == null ? "" : model.Column10;
                newmodel.Column11 = model.Column11 == null ? "" : model.Column11;
                newmodel.Column12 = model.Column12 == null ? "" : model.Column12;
                newmodel.Column13 = model.Column13 == null ? "" : model.Column13;
                newmodel.Column14 = model.Column14 == null ? "" : model.Column14;
                newmodel.Column15 = model.Column15 == null ? "" : model.Column15;
                newmodel.Column16 = model.Column16 == null ? "" : model.Column16;
                newmodel.Column17 = model.Column17 == null ? "" : model.Column17;
                newmodel.IsRSS = model.IsRSS;
                newmodel.IsShare = model.IsShare;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsForward = model.IsForward;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.ShowCount = model.ShowCount;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                newmodel.ColumnSetting = columnsetting;
                r = _unitsqlitemrepository.Update(newmodel);
            }
            if (r > 0)
            {
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }

        }
        #endregion

        //====

        #region GetModelByID
        public ActiveEditModel GetModelByID(string modelid, string itemid)
        {
            var data = _activeitemsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { modelid });
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var seodata = _seosqlrepository.GetByWhere("TypeName='ActiveItem' and TypeID=@1", new object[] { itemid });
                var model = new ActiveEditModel()
                {
                    ItemID = fdata.ItemID,
                    ModelName = maindata.Count() == 0 ? "" : maindata.First().Name,
                    Description = seodata.Count() > 0 ? seodata.First().Description : "",
                    ImageFileName = fdata.ImageFileName,
                    ImageFileOrgName = fdata.ImageFileOrgName,
                    WebsiteTitle = seodata.Count() > 0 ? seodata.First().Title : "",
                    Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10},
                    HtmlContent = fdata.HtmlContent,
                    ImageFileDesc = fdata.ImageFileDesc,
                    ImageFileLocation = fdata.ImageFileLocation,
                    LinkUrl = fdata.LinkUrl,
                    ModelID = fdata.ModelID.Value,
                    ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/ActiveItem/" + fdata.ImageFileName),
                    EdDate = fdata.EdDate,
                    EdDateStr = fdata.EdDate == null ? "" : fdata.EdDate.Value.ToString("yyyy/MM/dd"),
                    Group_ID = fdata.GroupID == null ? -1 : fdata.GroupID.Value,
                    Link_Mode = fdata.Link_Mode,
                    PublicshStr = fdata.PublicshDate == null ? "" : fdata.PublicshDate.Value.ToString("yyyy/MM/dd"),
                    StDate = fdata.StDate,
                    StDateStr = fdata.StDate == null ? "" : fdata.StDate.Value.ToString("yyyy/MM/dd"),
                    Title = fdata.Title,
                    CreateDatetime = fdata.CreateDatetime == null ? "" : fdata.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateUser = fdata.CreateName,
                    UpdateDatetime = fdata.UpdateDatetime == null ? "" : fdata.UpdateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateUser = fdata.UpdateName,
                    LinkUrlDesc = fdata.LinkUrlDesc,
                    RelateImageFileOrgName = fdata.RelateImageFileOrgName,
                    RelateImageName = fdata.RelateImageFileName,
                    RelateImagelUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/ActiveItem/" + fdata.RelateImageFileName),
                    ActiveDateStr = fdata.ActiveDate == null ? "" : fdata.ActiveDate.Value.ToString("yyyy/MM/dd"),
                     Contact = fdata.Contact == null ? "" : fdata.Contact,
                      ContactFax = fdata.ContactFax == null ? "" : fdata.ContactFax,
                       ContactMail = fdata.ContactMail == null ? "" : fdata.ContactMail,
                    EdDateNewStr = fdata.EdDateNew == null ? "" : fdata.EdDateNew.Value.ToString("yyyy/MM/dd"),
                     EdSignDateStr = fdata.EdSignDate == null ? "" : fdata.EdSignDate.Value.ToString("yyyy/MM/dd"),
                    Info = fdata.Info == null ? "" : fdata.Info,
                    Introduction = fdata.Introduction == null ? "" : fdata.Introduction,
                     IsNew= fdata.IsNew==null?false : fdata.IsNew.Value,
                      Location = fdata.Location == null ? "" : fdata.Location,
                       Organizer = fdata.Organizer == null ? "" : fdata.Organizer,
                    Price = fdata.Price == null ? "" : fdata.Price,
                    Search = fdata.Search == null ? "" : fdata.Search,
                     Source = fdata.Source == null ? "" : fdata.Source,
                      Way = fdata.Way == null ? "" : fdata.Way,
                       StDateNewStr = fdata.StDateNew == null ? "" : fdata.StDateNew.Value.ToString("yyyy/MM/dd"),
                        StSignDateStr = fdata.StSignDate == null ? "" : fdata.StSignDate.Value.ToString("yyyy/MM/dd"),
                         WebSite = fdata.WebSite == null ? "" : fdata.WebSite
                };
                var filedata = _ActiveFilessqlrepository.GetByWhere("ItemID=@1", new object[] { itemid }).OrderBy(v => v.FileIndex);
                var IMID = new List<string>();
                var IMDESC = new List<string>();
                var IMFilePath = new List<string>();
                var IMFileName = new List<string>();
                foreach (var a in filedata)
                {
                    IMID.Add(a.ID.ToString());
                    IMDESC.Add(a.FileDesc == null ? "" : a.FileDesc);
                    IMFileName.Add(a.FileName == null ? "" : a.FileName);
                    IMFilePath.Add(a.FilePath == null ? "" : a.FilePath);
                }
                model.UploadFileID = IMID.ToArray();
                model.UploadFileDesc = IMDESC.ToArray();
                model.UploadFilePath = IMFilePath.ToArray();
                model.UploadFileName = IMFileName.ToArray();

                return model;
            }
            else
            {
                ActiveEditModel model = new ActiveEditModel();
                model.Keywords = new string[10];
                model.ModelID = int.Parse(modelid);
                model.ImageFileLocation = "1";
                model.ModelName = maindata.Count() == 0 ? "" : maindata.First().Name;
                model.UploadFilePath = new string[] { };
                model.UploadFileName = new string[] { };
                model.UploadFileID = new string[] { };
                model.UploadFileDesc = new string[] { };
                return model;
            }

        }
        #endregion

        #region CreateItem
        public string CreateItem(ActiveEditModel model, string LangId, string account)
        {
            var iswriteseo = false;
            var olddata = _activeitemsqlrepository.GetByWhere("ModelID=@1", new object[] { model.ModelID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new ActiveItem()
            {
                HtmlContent = model.HtmlContent,
                ImageFileDesc = model.ImageFileDesc,
                ImageFileLocation = model.ImageFileLocation,
                ModelID = model.ModelID,
                ImageFileOrgName = model.ImageFileOrgName,
                LinkUrl = model.LinkUrl,
                ImageFileName = model.ImageFileName,
                Sort = 1,
                ClickCnt = 0,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(LangId),
                Title = model.Title,
                CreateDatetime = DateTime.Now,
                CreateUser = account,
                Enabled = true,
                Link_Mode = model.Link_Mode,
                RelateImageFileName = model.RelateImageName,
                RelateImageFileOrgName = model.RelateImageFileOrgName,
                Introduction = model.Introduction == null ? "" : model.Introduction,
                CreateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = true,
                LinkUrlDesc = model.LinkUrlDesc,
                Contact = model.Contact == null ? "" : model.Contact,
                 ContactFax = model.ContactFax == null ? "" : model.ContactFax,
                  ContactMail = model.ContactMail == null ? "" : model.ContactMail,
                   Info = model.Info == null ? "" : model.Info,
                    IsNew=model.IsNew,
                      Organizer = model.Organizer == null ? "" : model.Organizer,
                       Price = model.Price == null ? "" : model.Price,
                        Search = model.Search == null ? "" : model.Search,
                         Source = model.Source == null ? "" : model.Source,
                          WebSite = model.WebSite == null ? "" : model.WebSite,
                           Way = model.Way == null ? "" : model.Way,
                            Location = model.Location == null ? "" : model.Location
            };
            if (model.PublicshStr.IsNullorEmpty() == false)
            {
                savemodel.PublicshDate = DateTime.Parse(model.PublicshStr);
            }
            if (model.EdDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdDate = DateTime.Parse(model.EdDateStr);
            }
            if (model.StDateStr.IsNullorEmpty() == false)
            {
                savemodel.StDate = DateTime.Parse(model.StDateStr);
            }

            if (model.EdDateNewStr.IsNullorEmpty() == false)
            {
                savemodel.EdDateNew = DateTime.Parse(model.EdDateNewStr);
            }
            if (model.StDateNewStr.IsNullorEmpty() == false)
            {
                savemodel.StDateNew = DateTime.Parse(model.StDateNewStr);
            }

            if (model.StSignDateStr.IsNullorEmpty() == false)
            {
                savemodel.StSignDate = DateTime.Parse(model.StSignDateStr);
            }
            if (model.EdSignDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdSignDate = DateTime.Parse(model.EdSignDateStr);
            }
            if (model.ActiveDateStr.IsNullorEmpty() == false)
            {
                savemodel.ActiveDate = DateTime.Parse(model.ActiveDateStr);
            }

            var r = _activeitemsqlrepository.Create(savemodel);
            if (r > 0)
            {
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var basepath = uploadfilepath;
                if (System.IO.Directory.Exists(basepath + "\\Active") == false)
                {
                    System.IO.Directory.CreateDirectory(basepath + "\\Active");
                }
                if (model.UploadFileID != null)
                {
                    for (var idx = 0; idx < model.UploadFileID.Length; idx++)
                    {
                        var smodel = new ActiveFiles();
                        var pid = model.UploadFileID[idx];
                        smodel.FileDesc = model.UploadFileDesc[idx] == null ? "" : model.UploadFileDesc[idx];
                        smodel.FileIndex = idx;
                        smodel.LangID = savemodel.Lang_ID;
                        smodel.ItemID = savemodel.ItemID;
                        smodel.ModelID = savemodel.ModelID;
                        smodel.FileName = "";
                        if (pid == "-1")
                        {
                            if (model.UploadFile != null && model.UploadFile[idx] != null)
                            {
                                smodel.FileName = model.UploadFile[idx].FileName.Split('\\').Last();
                            }
                        }
                        r = _ActiveFilessqlrepository.Create(smodel);
                        if (pid == "-1")
                        {
                            if (model.UploadFile != null && model.UploadFile[idx] != null)
                            {
                                if (r > 0)
                                {
                                    var path = (smodel.ModelID) + "_" + (smodel.ItemID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.FileName.Split('.').Last();
                                    model.UploadFile[idx].SaveAs(basepath + "\\Active\\" + path);
                                    _ActiveFilessqlrepository.Update("FilePath=@1", "ID=@2", new object[] { "\\Active\\" + path, smodel.ID });
                                }
                            }
                        }
                        else
                        {
                            var oldmodel = _ActiveFilessqlrepository.GetByWhere("ID=@1", new object[] { pid }).First();
                            _ActiveFilessqlrepository.Update("FileName=@1", "ID=@2", new object[] { oldmodel.FileName, smodel.ID });
                            //複製檔案
                            var filepath = (smodel.ModelID) + "_" + (smodel.ItemID) + "_" + (smodel.ID) + "_" + (idx) + "." + oldmodel.FileName.Split('.').Last();
                            var copytopath = basepath + filepath;
                            var copyfrompath = basepath + oldmodel.FilePath;
                            System.IO.File.Copy(copyfrompath, copytopath, true);
                            _ActiveFilessqlrepository.Update("FileName=@1,FilePath=@2", "ID=@3", new object[] { oldmodel.FileName, "\\Active\\" + filepath, smodel.ID });
                        }
                    }
                }

                foreach (var odata in olddata)
                {
                    _activeitemsqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { odata.Sort + 1, odata.ItemID });
                }
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }

        }
        #endregion

        #region UpdateItem
        public string UpdateItem(ActiveEditModel model, string LangId, string account)
        {
            var iswriteseo = false;
            var olddata = _activeitemsqlrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
            var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
            if (uploadfilepath.IsNullorEmpty())
            {
                uploadfilepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadFile";
            }
            var basepath = uploadfilepath;
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ActiveItem\\";
            if (model.ImageFileName.IsNullorEmpty())
            {

                if (System.IO.File.Exists(oldroot + "\\" + olddata.First().ImageFileName))
                {
                    System.IO.File.Delete(oldroot + "\\" + olddata.First().ImageFileName);
                }
                model.ImageFileOrgName = "";
                model.ImageFileName = "";
            }
            else
            {

                if (olddata.First().ImageFileName != model.ImageFileName)
                {
                    if (System.IO.File.Exists(oldroot + "\\" + olddata.First().ImageFileName))
                    {
                        System.IO.File.Delete(oldroot + "\\" + olddata.First().ImageFileName);
                    }
                }
            }

            if (model.RelateImageName.IsNullorEmpty())
            {

                if (System.IO.File.Exists(oldroot + "\\" + olddata.First().RelateImageFileName))
                {
                    System.IO.File.Delete(oldroot + "\\" + olddata.First().RelateImageFileName);
                }
                model.RelateImageFileOrgName = "";
                model.RelateImageName = "";
            }
            else
            {

                if (olddata.First().RelateImageFileName != model.RelateImageName)
                {
                    if (System.IO.File.Exists(oldroot + "\\" + olddata.First().RelateImageFileName))
                    {
                        System.IO.File.Delete(oldroot + "\\" + olddata.First().RelateImageFileName);
                    }
                }
            }


            _seosqlrepository.DelDataUseWhere("TypeName='ActiveItem' and TypeID=@1", new object[] { model.ItemID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new ActiveItem()
            {
                ItemID = model.ItemID,
                HtmlContent = model.HtmlContent == null ? "" : model.HtmlContent,
                ImageFileDesc = model.ImageFileDesc == null ? "" : model.ImageFileDesc,
                ImageFileLocation = model.ImageFileLocation == null ? "" : model.ImageFileLocation,
                RelateImageFileName = model.RelateImageName,
                RelateImageFileOrgName = model.RelateImageFileOrgName,
                ModelID = model.ModelID,
                ImageFileOrgName = model.ImageFileOrgName,
                LinkUrl = model.LinkUrl == null ? "" : model.LinkUrl == null ? "" : model.LinkUrl,
                LinkUrlDesc = model.LinkUrlDesc == null ? "" : model.LinkUrlDesc == null ? "" : model.LinkUrlDesc,
                ImageFileName = model.ImageFileName,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(LangId),
                Title = model.Title == null ? "" : model.Title,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                Enabled = true,
                Link_Mode = model.Link_Mode,
                Introduction = model.Introduction == null ? "" : model.Introduction,
                UpdateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = true,
                Contact = model.Contact == null ? "" : model.Contact,
                ContactFax = model.ContactFax == null ? "" : model.ContactFax,
                ContactMail = model.ContactMail == null ? "" : model.ContactMail,
                Info = model.Info == null ? "" : model.Info,
                IsNew = model.IsNew,
                Organizer = model.Organizer == null ? "" : model.Organizer,
                Price = model.Price == null ? "" : model.Price,
                Search = model.Search == null ? "" : model.Search,
                Source = model.Source == null ? "" : model.Source,
                WebSite = model.WebSite == null ? "" : model.WebSite,
                Way = model.Way == null ? "" : model.Way,
                Location = model.Location == null ? "" : model.Location,
                
            };
            if (model.PublicshStr.IsNullorEmpty() == false)
            {
                savemodel.PublicshDate = DateTime.Parse(model.PublicshStr);
            }
            if (model.EdDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdDate = DateTime.Parse(model.EdDateStr);
            }
            if (model.EdDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdDate = DateTime.Parse(model.EdDateStr);
            }
            if (model.StDateStr.IsNullorEmpty() == false)
            {
                savemodel.StDate = DateTime.Parse(model.StDateStr);
            }
            if (model.EdDateNewStr.IsNullorEmpty() == false)
            {
                savemodel.EdDateNew = DateTime.Parse(model.EdDateNewStr);
            }
            if (model.StDateNewStr.IsNullorEmpty() == false)
            {
                savemodel.StDateNew = DateTime.Parse(model.StDateNewStr);
            }

            if (model.StSignDateStr.IsNullorEmpty() == false)
            {
                savemodel.StSignDate = DateTime.Parse(model.StSignDateStr);
            }
            if (model.EdSignDateStr.IsNullorEmpty() == false)
            {
                savemodel.EdSignDate = DateTime.Parse(model.EdSignDateStr);
            }
            if (model.ActiveDateStr.IsNullorEmpty() == false)
            {
                savemodel.ActiveDate = DateTime.Parse(model.ActiveDateStr);
            }
            var r = _activeitemsqlrepository.Update(savemodel);
            if (r > 0)
            {
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                var oldfilesdata = _ActiveFilessqlrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
                if (model.UploadFileID == null) { model.UploadFileID = new string[] { }; }
                var delfile = oldfilesdata.Where(v => model.UploadFileID.Contains(v.ID.ToString()) == false);
                if (model.UploadFileID != null)
                {
                    if (System.IO.Directory.Exists(basepath + "\\Active") == false)
                    {
                        System.IO.Directory.CreateDirectory(basepath + "\\Active");
                    }
                    for (var idx = 0; idx < model.UploadFileID.Length; idx++)
                    {
                        var smodel = new ActiveFiles();
                        var pid = model.UploadFileID[idx];
                        smodel.FileDesc = model.UploadFileDesc[idx] == null ? "" : model.UploadFileDesc[idx];
                        smodel.FileIndex = idx;
                        smodel.LangID = savemodel.Lang_ID;
                        smodel.ItemID = model.ItemID;
                        smodel.ModelID = model.ModelID;
                        if (model.UploadFileID[idx] == "-1")
                        {
                            if (model.UploadFile[idx] != null)
                            {
                                smodel.FileName = model.UploadFile[idx].FileName.Split('\\').Last();
                            }
                            r = _ActiveFilessqlrepository.Create(smodel);
                            if (model.UploadFile[idx] != null)
                            {
                                if (r > 0)
                                {
                                    var path = (smodel.ModelID) + "_" + (smodel.ItemID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.FileName.Split('.').Last();
                                    model.UploadFile[idx].SaveAs(basepath + "\\Active\\" + path);
                                    _ActiveFilessqlrepository.Update("FilePath=@1", "ID=@2", new object[] { "\\Active\\" + path, smodel.ID });
                                }
                            }
                        }
                        else
                        {
                            var oldfiledata = oldfilesdata.Where(v => v.ID == int.Parse(model.UploadFileID[idx]));
                            if (oldfiledata.Count() > 0)
                            {
                                smodel.ID = oldfiledata.First().ID;
                                if (model.UploadFile[idx] != null)
                                {
                                    //刪除舊檔案
                                    if (System.IO.File.Exists(basepath + "\\Active\\" + oldfiledata.First().FilePath))
                                    {
                                        System.IO.File.Delete(basepath + "\\Active\\" + oldfiledata.First().FilePath);
                                    }
                                    smodel.FileName = model.UploadFile[idx].FileName.Split('\\').Last();
                                    r = _ActiveFilessqlrepository.Update(smodel);
                                    if (model.UploadFile[idx] != null)
                                    {
                                        if (r > 0)
                                        {
                                            var path = (smodel.ModelID) + "_" + (smodel.ItemID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.FileName.Split('.').Last();
                                            model.UploadFile[idx].SaveAs(basepath + "\\Active\\" + path);
                                            _ActiveFilessqlrepository.Update("FilePath=@1", "ID=@2", new object[] { "\\Active\\" + path, smodel.ID });
                                        }
                                    }
                                }
                                else
                                {
                                    if (model.UploadFilePath[idx] == "")
                                    {
                                        if (System.IO.File.Exists(basepath + "\\Active\\" + oldfiledata.First().FilePath))
                                        {
                                            System.IO.File.Delete(basepath + "\\Active\\" + oldfiledata.First().FilePath);
                                        }
                                        smodel.FileName = "";
                                        smodel.FilePath = "";
                                    }
                                    else
                                    {
                                        smodel.FileName = oldfiledata.First().FileName;
                                        smodel.FilePath = oldfiledata.First().FilePath;
                                    }
                                    r = _ActiveFilessqlrepository.Update(smodel);
                                }
                                oldfilesdata = oldfilesdata.Where(v => v.ID != smodel.ID);
                            }
                            else
                            {
                                if (model.UploadFile[idx] != null)
                                {
                                    smodel.FileName = model.UploadFile[idx].FileName.Split('\\').Last();
                                }
                                r = _ActiveFilessqlrepository.Create(smodel);
                                if (model.UploadFile[idx] != null)
                                {
                                    if (r > 0)
                                    {
                                        var path = (smodel.ModelID) + "_" + (smodel.ItemID) + "_" + (smodel.ID) + "_" + (idx) + "." + smodel.FileName.Split('.').Last();
                                        model.UploadFile[idx].SaveAs(basepath + "\\Active\\" + path);
                                        _ActiveFilessqlrepository.Update("FilePath=@1", "ID=@2", new object[] { "\\Active\\" + path, smodel.ID });
                                    }
                                }
                            }
                        }
                    }
                }
                foreach (var del in delfile) {

                    _ActiveFilessqlrepository.DelDataUseWhere("ID=@1", new object[] { del.ID });
                    if (System.IO.File.Exists(basepath + "\\Active\\" + del.FilePath))
                    {
                        System.IO.File.Delete(basepath + "\\Active\\" + del.FilePath);
                    }

                }

                if (iswriteseo)
                {
                    r = _seosqlrepository.Create(new SEO()
                    {
                        Description = model.Description == null ? "" : model.Description,
                        Keywords1 = model.Keywords[0],
                        Keywords2 = model.Keywords[1],
                        Keywords3 = model.Keywords[2],
                        Keywords4 = model.Keywords[3],
                        Keywords5 = model.Keywords[4],
                        Keywords6 = model.Keywords[5],
                        Keywords7 = model.Keywords[6],
                        Keywords8 = model.Keywords[7],
                        Keywords9 = model.Keywords[8],
                        Keywords10 = model.Keywords[9],
                        Title = model.WebsiteTitle == null ? "" : model.WebsiteTitle,
                        TypeName = "ActiveItem",
                        TypeID = savemodel.ItemID,
                        Lang_ID = int.Parse(LangId)
                    });
                }
                return "修改成功";
            }
            else
            {
                return "修改失敗";
            }
        }

        #endregion

        #region PagingItem
        public Paging<ActiveItemResult> PagingItem(string modelid, ActiveSearchModel model)
        {
            var Paging = new Paging<ActiveItemResult>();
            var data = new List<ActiveItem>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(modelid);
            if (model.GroupId != null)
            {
                idx += 1;
                wherestr.Add("GroupID=@" + idx);
                whereobj.Add(model.GroupId.Value.ToString());
            }
            if (string.IsNullOrEmpty(model.Title) == false)
            {
                idx += 1;
                wherestr.Add("(Title like @" + idx + " or HtmlContent like @" + idx + ")");
                whereobj.Add("%" + model.Title + "%");
            }

            if (model.Enabled != null)
            {
                idx += 1;
                wherestr.Add("Enabled=@" + idx);
                whereobj.Add(model.Enabled);
            }

            if (string.IsNullOrEmpty(model.PublicshDateFrom) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate >=@" + idx + " or PublicshDate=Null)");
                whereobj.Add(model.PublicshDateFrom);
            }
            if (string.IsNullOrEmpty(model.PublicshDateTo) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate <=@" + idx + " or  PublicshDate=Null)");
                whereobj.Add(model.PublicshDateTo);
            }

            if (string.IsNullOrEmpty(model.DisplayFrom) == false)
            {
                idx += 1;
                wherestr.Add("(EdDate IS NULL OR EdDate >=@" + idx + ")");
                whereobj.Add(model.DisplayFrom);
            }
            if (string.IsNullOrEmpty(model.DisplayTo) == false)
            {
                idx += 1;
                wherestr.Add("(StDate IS NULL OR StDate <=@" + idx + ")");
                whereobj.Add(model.DisplayTo);
            }
            if (string.IsNullOrEmpty(model.IsRange) == false)
            {
                idx += 1;
                if (model.IsRange == "1")
                {
                    wherestr.Add("((StDate <=@" + idx + " or StDate is null or StDate='') and (EdDate >=@" + idx + " or EdDate is null or EdDate=''))");
                    whereobj.Add(DateTime.Now);
                }
                else
                {
                    wherestr.Add("((StDate >@" + idx + ") or (EdDate <@" + idx + "))");
                    whereobj.Add(DateTime.Now);
                }
            }

            if (model.Limit!=-1)
            {
                var str = string.Join(" and ", wherestr);
                data = _activeitemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _activeitemsqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                Paging.total = _activeitemsqlrepository.GetAllDataCount();
                var str = string.Join(" and ", wherestr);
                data = _activeitemsqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
            }

            var ALLGroup = _groupsqlrepository.GetAll();
            foreach (var d in data)
            {
                var isrange = false;
                if (d.StDate == null && d.EdDate == null) { isrange = true; }
                else if (d.StDate != null && d.EdDate == null)
                {
                    if (DateTime.Now >= d.StDate.Value)
                    {
                        isrange = true;
                    }
                }
                else if (d.StDate == null && d.EdDate != null)
                {
                    if (DateTime.Now <= d.EdDate.Value.AddDays(1))
                    {
                        isrange = true;
                    }
                }
                else if (d.StDate != null && d.EdDate != null)
                {
                    if (DateTime.Now >= d.StDate.Value && DateTime.Now <= d.EdDate.Value.AddDays(1))
                    {
                        isrange = true;
                    }
                }
                var IsActive = false;
                if (d.StSignDate == null && d.EdSignDate == null) { IsActive = true; }
                else if (d.StSignDate != null && d.EdSignDate == null)
                {
                    if (DateTime.Now >= d.StSignDate)
                    {
                        IsActive = true;
                    }
                }
                else if (d.StSignDate == null && d.EdSignDate != null)
                {
                    if (DateTime.Now <= d.EdSignDate.Value.AddDays(1))
                    {
                        IsActive = true;
                    }
                }
                else if (d.StSignDate != null && d.EdSignDate != null)
                {
                    if (DateTime.Now >= d.StSignDate.Value && DateTime.Now <= d.EdSignDate.Value.AddDays(1))
                    {
                        IsActive = true;
                    }
                }

                var gname = ALLGroup.Where(v => v.ID == d.GroupID);
                Paging.rows.Add(new ActiveItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    IsRange = isrange == true ? "是" : "否",
                    GroupName = gname.Count() > 0 ? gname.First().Group_Name : "無分類",
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy/MM/dd"),
                    Sort = d.Sort.Value,
                    IsActive= IsActive==true? "報名中" : "報名截止"
                });

            }
            return Paging;
        }
        #endregion

        #region UpdateItemSeq
        public string UpdateItemSeq(int modelid, int id, int seq, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _activeitemsqlrepository.GetByWhere("ItemID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ActiveItem> mtseqdata = null;
                    mtseqdata = _activeitemsqlrepository.GetByWhere("Sort>@1 and ModelID=@2", new object[] { seq, modelid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _activeitemsqlrepository.GetCountUseWhere("ModelID=@1", new object[] { modelid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ActiveItem> ltseqdata = null;
                    ltseqdata = _activeitemsqlrepository.GetByWhere("Sort<=@1 and ModelID=@2", new object[] { qseq, modelid }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.ItemID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _activeitemsqlrepository.Update(tempmodel);
                    }
                }

                oldmodel.First().Sort = seq;
                r = _activeitemsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新ActiveItem排序失敗:" + " error:" + ex.Message);
                return "更新ActiveItem排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region DeleteItem
        public string DeleteItem(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var uploadfilepath = ConfigurationManager.AppSettings["UploadFile"];
                if (uploadfilepath.IsNullorEmpty())
                {
                    uploadfilepath = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadFile";
                }
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ActiveItem\\";
                var modelid = -1;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ActiveItem();
                    var olditem = _activeitemsqlrepository.GetByWhere("ItemID=@1", new object[] { idlist[idx] });
                    entity.ItemID = int.Parse(idlist[idx]);
                    modelid = olditem.First().ModelID.Value;
                    r = _activeitemsqlrepository.Delete(entity);
                    _ActiveDateRangelitemrepository.DelDataUseWhere("ItemID=@1", new object[] { idlist[idx] });
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除群組失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        _seosqlrepository.DelDataUseWhere("TypeName='ActiveItem' and TypeID=@1", new object[] { idlist[idx] });
                        if (olditem.First().UploadFileName.IsNullorEmpty() == false)
                        {
                            if (System.IO.File.Exists(olditem.First().UploadFilePath))
                            {
                                System.IO.File.Delete(olditem.First().UploadFilePath);
                            }
                        }
                        if (olditem.First().ImageFileName.IsNullorEmpty() == false)
                        {

                            if (System.IO.File.Exists(oldroot + "\\" + olditem.First().ImageFileName))
                            {
                                System.IO.File.Delete(oldroot + "\\" + olditem.First().ImageFileName);
                            }
                        }
                        var filedata = _ActiveFilessqlrepository.GetByWhere("ItemID=@1", new object[] { idlist[idx] });
                        foreach (var f in filedata) {
                            if (System.IO.File.Exists(uploadfilepath + f.FilePath)) {
                                System.IO.File.Delete(uploadfilepath + f.FilePath);
                            }
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                    NLogManagement.SystemLogInfo("刪除群組:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _activeitemsqlrepository.GetByWhere("ModelId=@1", new object[] { modelid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    _activeitemsqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { idx, tempmodel.ItemID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region SetItemStatus
        public string SetItemStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new ActiveItem();
                entity.Enabled = status ? true : false;
                entity.ItemID = int.Parse(id);
                var r = _activeitemsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改ActiveItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改ActiveItem顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region AddPhoto
        public string AddPhoto(AddPhotoModel model, string uploadfilepath, string user)
        {
            var maxsort = _photosqlrepository.GetDataCaculate("Max(Sort)", "ItemID=@1 AND MainID=@2", new object[] { model.ItemID, model.MainID });
            if (model.files.Count() > 0)
            {
                for (var idx = 0; idx < model.files.Length; idx++)
                {
                    if (model.files[idx] != null)
                    {
                        maxsort += 1;
                        var UploadFileName = model.files[idx].FileName.Split('\\').Last();

                        var filename = DateTime.Now.Ticks + idx.ToString() + "." + model.files[idx].FileName.Split('.').Last();
                        var path = uploadfilepath + "UploadImage\\ActivePhoto\\" + model.ItemID + "\\";
                        if (System.IO.Directory.Exists(path) == false)
                        {
                            System.IO.Directory.CreateDirectory(path);
                        }
                        model.files[idx].SaveAs(path + "\\" + filename);
                        _photosqlrepository.Create(new ActivePhoto()
                        {
                            Enabled = true,
                            FileDesc = model.text[idx] == null ? "" : model.text[idx],
                            MainID = model.MainID,
                            ItemID = model.ItemID,
                            Sort = maxsort,
                            FileName = filename,
                            FilePath = path,
                            UpdateDatetime = DateTime.Now,
                            UpdateUser = user
                        });
                    }
                }
            }
            else
            {
                return "沒有上傳的檔案";
            }

            return "上傳結束";
        }
        #endregion

        #region PagingPhoto
        public Paging<ActivePhoto> PagingPhoto(SearchModelBase model)
        {
            var Paging = new Paging<ActivePhoto>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("ItemID=" + model.ModelID);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("FileName like @1");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            // VirtualPathUtility.ToAbsolute("~/UploadImage/MessageItem/" + fdata.ImageFileName)
            Paging.rows = _photosqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            foreach (var v in Paging.rows)
            {
                v.FilePath = VirtualPathUtility.ToAbsolute("~/UploadImage/ActivePhoto/" + v.ItemID + "/" + v.FileName);
            }
            Paging.total = _photosqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region UpdatePhotoSeq
        public string UpdatePhotoSeq(int id, int seq, string itemid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _photosqlrepository.GetByWhere("PID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ActivePhoto> mtseqdata = null;
                    mtseqdata = _photosqlrepository.GetByWhere("Sort>@1 and ItemID=@2", new object[] { seq, itemid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _photosqlrepository.GetCountUseWhere("ItemID=@1", new object[] { itemid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ActivePhoto> ltseqdata = null;
                    ltseqdata = _photosqlrepository.GetByWhere("Sort<=@1  and ItemID=@2", new object[] { qseq, itemid }).OrderBy(v => v.Sort).ToArray();

                    var sidx = 0;
                    for (var idx = 1; idx <= ltseqdata.Count(); idx++)
                    {
                        if (idx == seq && seq < oldmodel.First().Sort)
                        {
                            sidx += 1;
                        }
                        var tempmodel = ltseqdata[idx - 1];
                        if (tempmodel.PID == id)
                        {
                            continue;
                        }
                        else
                        {
                            sidx += 1;
                        }
                        tempmodel.Sort = sidx;
                        _photosqlrepository.Update(tempmodel);
                    }
                }

                oldmodel.First().Sort = seq;
                r = _photosqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新ActivePhoto排序失敗:" + " error:" + ex.Message);
                return "更新ActivePhoto排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region UpdatePhotoStatus
        public string UpdatePhotoStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new ActivePhoto();
                entity.Enabled = status ? true : false;
                entity.UpdateDatetime = DateTime.Now;
                entity.PID = int.Parse(id);
                entity.UpdateUser = account;
                var r = _photosqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改ActivePhoto顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改ActivePhoto顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region DeletePhoto
        public string DeletePhoto(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var mainid = -1;
                var itemid = -1;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var olddata = _photosqlrepository.GetByWhere("PID=@1", new object[] { idlist[idx] });
                    mainid = olddata.First().MainID.Value;
                    itemid = olddata.First().ItemID.Value;
                    var entity = new ActivePhoto();
                    entity.PID = int.Parse(idlist[idx]);
                    r = _photosqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除群組失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        if (System.IO.File.Exists(olddata.First().FilePath))
                        {
                            System.IO.File.Delete(olddata.First().FilePath);
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除群組:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _photosqlrepository.GetByWhere("MainID=@1 and ItemID=@2", new object[] { mainid, itemid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    _photosqlrepository.Update("Sort=@1", "PID=@2", new object[] { idx, tempmodel.PID });
                }

                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region UpdatePhotoDesc
        public string UpdatePhotoDesc(Dictionary<string, string> model)
        {
            try
            {

                foreach (var key in model.Keys)
                {
                    var r = _photosqlrepository.Update("FileDesc=@1", "PID=@2", new object[] { model[key], key });

                }
                return "更新成功";
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改ActivePhoto顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region GetActiveItemList
        public List<ActivePhoto> GetActiveItemList(string ItemID)
        {
            var data = _photosqlrepository.GetByWhere("ItemID=@1 Order By Sort", new object[] { ItemID });
            if (data.Count() > 0) { return data.ToList(); }
            else
            {
                return new List<ActivePhoto>();
            }
        }
        #endregion

        #region GetModelItem
        public ActiveItem GetModelItem(string itemid)
        {
            var data = _activeitemsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            if (data.Count() > 0)
            {
                return data.First();
            }
            else
            {
                return new ActiveItem();
            }
        }
        #endregion

        #region GetGroupName
        public string GetGroupName(string groupid)
        {
            var data = _groupsqlrepository.GetByWhere("ID=@1", new object[] { groupid });
            if (data.Count() > 0)
            {
                return data.First().Group_Name;
            }
            else
            {
                return "";
            }
        }
        #endregion

        #region UpdateClickCount
        public void UpdateClickCount(string itemid)
        {
            string tClientIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            var HTTP_VIA = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"];
            if (HTTP_VIA.IsNullorEmpty() == false)
            {
                var viaip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (viaip.IsNullorEmpty() == false)
                {
                    tClientIP = viaip;
                }
            }
            _clicktablesqlitemrepository.DelDataUseWhere("Type='Active' and LastClickDatetime<@1", new object[] { DateTime.Now.AddDays(-1) });
            var hasdata = _clicktablesqlitemrepository.GetByWhere("Type='Active' and IP=@1 and ID=@2", new object[] { tClientIP, itemid });
            if (hasdata.Count() == 0)
            {
                _activeitemsqlrepository.Update("ClickCnt=ClickCnt+1", "ItemId=@1", new object[] { itemid });
                _clicktablesqlitemrepository.Create(new ClickCountTable()
                {
                    IP = tClientIP,
                    LastClickDatetime = DateTime.Now,
                    Type = "Active",
                    ID = itemid
                });
            }

            _activeitemsqlrepository.Update("ClickCnt=ClickCnt+1", "ItemID=@1", new object[] { itemid });
        }
        #endregion

        #region PagingItemForWebSite
        public Paging<ActiveItemResult> PagingItemForWebSite(string modelid, ActiveSearchModel model, string nogroupstr)
        {
            var Paging = new Paging<ActiveItemResult>();
            var data = new List<ActiveItem>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(modelid);
            var nowdate = DateTime.Now.ToString("yyyy/MM/dd");

            idx += 1;
            wherestr.Add("Enabled=@" + idx);
            whereobj.Add(true);

            idx += 1;
            wherestr.Add("(StDate <=@" + idx + " or StDate is null or StDate='')");
            whereobj.Add(nowdate);

            idx += 1;
            wherestr.Add("(EdDate >=@" + idx + " or EdDate is null or EdDate='')");
            whereobj.Add(nowdate);
            wherestr.Add("IsVerift=1");

            if (string.IsNullOrEmpty(model.DisplayTo) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate <=@" + idx + " or PublicshDate is null or PublicshDate='')");
                whereobj.Add(model.DisplayTo);
            }
            if (string.IsNullOrEmpty(model.DisplayFrom) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate >=@" + idx + " or PublicshDate is null or PublicshDate='')");
                whereobj.Add(model.DisplayFrom);
            }


            if (string.IsNullOrEmpty(model.Title) == false)
            {
                idx += 1;
                wherestr.Add("(Title like @" + idx + " or HtmlContent like @" + idx + ")");
                whereobj.Add("%" + model.Title + "%");
            }
            var ALLGroup = _groupsqlrepository.GetByWhere("Main_ID=@1 and Enabled=@2", new object[] { modelid, 1 });
            if (model.GroupId != null)
            {
                idx += 1;
                wherestr.Add("GroupID=@" + idx);
                whereobj.Add(model.GroupId.Value.ToString());
            }
            else
            {
                if (ALLGroup.Count() > 0)
                {
                    var enabled = ALLGroup.Where(v => v.Enabled == true).Select(v => v.ID.ToString());
                    var instr = string.Join(",", enabled);
                    wherestr.Add("GroupID In(" + instr + ",0)");
                }
                else
                {
                    wherestr.Add("GroupID=0");
                }
            }

            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                if (model.Limit == -1)
                {
                    data = _activeitemsqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
                }
                else
                {
                    data = _activeitemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                }

                Paging.total = _activeitemsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            }
            else
            {
                if (model.Limit == -1)
                {
                    data = _activeitemsqlrepository.GetAll("*", model.Sort).ToList();
                }
                else
                {
                    data = _activeitemsqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
                }
                Paging.total = _activeitemsqlrepository.GetAllDataCount();

            }
            var sidx = model.Offset;
            foreach (var d in data)
            {

                var gname = ALLGroup.Where(v => v.ID == d.GroupID);
                var isnew = false;
                if (d.IsNew.Value) {
                    if (d.StDateNew == null && d.EdDateNew == null) { isnew = true; }
                    else if (d.StDateNew != null && d.EdDateNew == null)
                    {
                        if (DateTime.Now >= d.StDateNew.Value)
                        {
                            isnew = true;
                        }
                    }
                    else if (d.StDateNew == null && d.EdDateNew != null)
                    {
                        if (DateTime.Now <= d.EdDateNew.Value.AddDays(1))
                        {
                            isnew = true;
                        }
                    }
                    else if (d.StDateNew != null && d.EdDateNew != null)
                    {
                        if (DateTime.Now >= d.StDateNew.Value && DateTime.Now <= d.EdDateNew.Value.AddDays(1))
                        {
                            isnew = true;
                        }
                    }
                }
                sidx += 1;
                Paging.rows.Add(new ActiveItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    GroupName = gname.Count() > 0 ? gname.First().Group_Name : nogroupstr,
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy.MM.dd"),
                    Sort = sidx,
                    LinkUrl = d.LinkUrl == null ? "" : d.LinkUrl,
                    UploadFileName = d.UploadFileName == null ? "" : d.UploadFileName,
                    Link_Mode = d.Link_Mode,
                    RelatceImageFileName = d.RelateImageFileName == null ? "" : d.RelateImageFileName,
                    IsNew= isnew
                });

            }
            return Paging;
        }
        #endregion

        #region GetAllGroupSelectList
        public IList<SelectListItem> GetAllGroupSelectList(string mainid)
        {
            var list = _groupsqlrepository.GetByWhere("Main_ID=@1 Order By Sort", new object[] { mainid });
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            item.Add(new System.Web.Mvc.SelectListItem() { Text = "無分類", Value = "0" });
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Group_Name, Value = l.ID.ToString() });
            }
            return item;
        }

        #endregion

        #region GetActivePhotoCount
        public int GetActivePhotoCount(string ItemID)
        {
            var data = _photosqlrepository.GetDataCaculate("Count(ItemID)", "ItemID=@1", new object[] { ItemID });
            return data;
        }
        #endregion

        #region GetModelDataRange
        public ActiveDateRange[] GetModelDataRange(string itemid)
        {
            var data = _ActiveDateRangelitemrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            return data.ToArray();
        }
        #endregion

        #region GetActiveFiles
        public ActiveFiles GetActiveFiles(string fileid)
        {
            var files = _ActiveFilessqlrepository.GetByWhere("ID=@1", new object[] { fileid });
            if (files.Count() > 0)
            {
                return files.First();
            }
            else
            {
                return new ActiveFiles();
            }
        }
        #endregion

        #region GetAllActiveFiles
        public ActiveFiles[] GetAllActiveFiles(string itemid)
        {
            return  _ActiveFilessqlrepository.GetByWhere("ItemID=@1", new object[] { itemid }).ToArray();
        }
        #endregion

        #region GetGroupSelectList
        public IList<SelectListItem> GetFrontGroupSelectList(string mainid)
        {
            var list = _groupsqlrepository.GetByWhere("Enabled=@1 and Main_ID=@2 Order By Sort", new object[] { true, mainid });
            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(mainid);
            var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
            idx += 1;
            wherestr.Add("Enabled=@" + idx);
            whereobj.Add(true);
            idx += 1;
            wherestr.Add("(StDate <=@" + idx + " or StDate is null or StDate='')");
            whereobj.Add(nowdate);
            idx += 1;
            wherestr.Add("(EdDate >=@" + idx + " or EdDate is null or EdDate='')");
            whereobj.Add(nowdate);
            var str = string.Join(" and ", wherestr);
            var usrid = _activeitemsqlrepository.GetByWhere(str, whereobj.ToArray()).Select(v => v.GroupID).ToArray();
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            item.Add(new System.Web.Mvc.SelectListItem() { Text = "全部", Value = "" });
            foreach (var l in list)
            {
                if (usrid.Contains(l.ID))
                {
                    item.Add(new System.Web.Mvc.SelectListItem() { Text = l.Group_Name, Value = l.ID.ToString() });
                }
            }
            return item;
        }
        #endregion
    }
}
