using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLModel.Models;
using ViewModels;
using SQLModel;
using Utilities;
using System.Web.Mvc;
using System.Web;
using System.ServiceModel.Syndication;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class ModelArticleManager : IModelArticleManager
    {
        readonly SQLRepository<ModelArticleMain> _sqlrepository;
        readonly SQLRepository<GroupArticle> _groupsqlrepository;
        readonly SQLRepository<ArticleItem> _sqlitemrepository;
        readonly SQLRepository<ArticleUnitSetting> _unitsqlitemrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<ColumnSetting> _columnsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<PageLayout> _pagesqlrepository;
        readonly SQLRepository<ClickCountTable> _clicktablesqlitemrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        readonly SQLRepository<GroupItemArticle> _groupsubsqlrepository;
        readonly SQLRepository<ArticelGroupItem> _articelgroupsqlrepository;
        readonly SQLRepository<SiteConfig> _siteconfigqlrepository;
        public ModelArticleManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelArticleMain;
            _groupsqlrepository = sqlinstance.GroupArticle;
            _seosqlrepository = sqlinstance.SEO;
            _sqlitemrepository = sqlinstance.ArticleItem;
            _unitsqlitemrepository = sqlinstance.ArticleUnitSetting;
            _columnsqlrepository = sqlinstance.ColumnSetting;
            _menusqlrepository = sqlinstance.Menu;
            _pagesqlrepository = sqlinstance.PageLayout;
            _clicktablesqlitemrepository = sqlinstance.ClickCountTable;
            _Usersqlrepository = sqlinstance.Users;
            _groupsubsqlrepository = sqlinstance.GroupItemArticle;
            _articelgroupsqlrepository = sqlinstance.ArticelGroupItem;
            _siteconfigqlrepository = sqlinstance.SiteConfig;
        }

        #region GetAll
        public IEnumerable<ModelArticleMain> GetAll()
        {
            return _sqlrepository.GetAll();
        }

        #endregion

        #region Paging
        public Paging<ModelArticleMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelArticleMain>();
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
            return Paging;
        }
        #endregion

        #region Where
        public IEnumerable<ModelArticleMain> Where(ModelArticleMain model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region AddUnit
        public string AddUnit(string name, string langid, string account,ref int newid)
        {
            var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 Order By Sort", new object[] { langid });
            var idx = 2;
            foreach (var mdata in alldata)
            {
                mdata.Sort = idx;
                _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
                idx += 1;
            }
            var Model = new ModelArticleMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 7;
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
                    IList<ModelArticleMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and Lang_ID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelArticleMain> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1  and Lang_ID=@2", new object[] { qseq, langid }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
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
                //先取出大於目前seq的資料

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
                NLogManagement.SystemLogInfo("更新ModelArticleMain排序失敗:" + " error:" + ex.Message);
                return "更新ModelArticleMain排序失敗:" + " error:" + ex.Message;
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
                    IList<GroupArticle> mtseqdata = null;
                    mtseqdata = _groupsqlrepository.GetByWhere("Sort>@1 and Main_ID=@2", new object[] { seq, mainid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _groupsqlrepository.GetCountUseWhere("Main_ID=@1", new object[] { mainid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<GroupArticle> ltseqdata = null;
                    ltseqdata = _groupsqlrepository.GetByWhere("Sort<=@1  and Main_ID=@2", new object[] { qseq, mainid }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
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

        #region UpdateGroupSubSeq
        public string UpdateGroupSubSeq(int id, int seq, string mainid, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _groupsubsqlrepository.GetByWhere("ID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<GroupItemArticle> mtseqdata = null;
                    mtseqdata = _groupsubsqlrepository.GetByWhere("Sort>@1 and Group_ID=@2", new object[] { seq, mainid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _groupsubsqlrepository.GetCountUseWhere("Group_ID=@1", new object[] { mainid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<GroupItemArticle> ltseqdata = null;
                    ltseqdata = _groupsubsqlrepository.GetByWhere("Sort<=@1  and Group_ID=@2", new object[] { qseq, mainid }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
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
                        _groupsubsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _groupsubsqlrepository.Update(oldmodel.First());
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

        #region SetGroupSubDelete
        public string SetGroupSubDelete(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\Article\\";

                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var fname = _groupsubsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().ImageFileName;
                    var entity = new GroupItemArticle();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _groupsubsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除群組失敗:ID=" + idlist[idx]);
                    }
                    if (System.IO.File.Exists(oldroot + "\\" + fname))
                    {
                        System.IO.File.Delete(oldroot + "\\" + fname);
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
                var alldata = _groupsubsqlrepository.GetAll().OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _groupsubsqlrepository.Update(tempmodel);
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

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var m = _menusqlrepository.GetByWhere("ModelID=7 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                    var p = _pagesqlrepository.GetByWhere("ModelID=7 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (p.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ArticleItem\\";
                var oldroot2 = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\Article\\";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelArticleMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除訊息管理單元失敗:ID=" + idlist[idx]);
                    }
                    else {
                        var allitems = _sqlitemrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        _seosqlrepository.DelDataUseWhere("TypeName='Article' and TypeID=@1", new object[] { idlist[idx] });
                        var group = _groupsqlrepository.GetByWhere("Main_ID=@1", new object[] { idlist[idx] });
                        _groupsqlrepository.DelDataUseWhere("Main_ID=@1", new object[] { idlist[idx] });
                        foreach (var G in group) {
                            if (System.IO.File.Exists(oldroot2 + "\\" + G.ImageFileName))
                            {
                                System.IO.File.Delete(oldroot2 + "\\" + G.ImageFileName);
                            }
                        }
                        _articelgroupsqlrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });
                        var subgroup = _groupsubsqlrepository.GetByWhere("Main_ID=@1", new object[] { idlist[idx] });
                        _groupsubsqlrepository.DelDataUseWhere("Main_ID=@1", new object[] { idlist[idx] });
                        foreach (var G in subgroup)
                        {
                            if (System.IO.File.Exists(oldroot2 + "\\" + G.ImageFileName))
                            {
                                System.IO.File.Delete(oldroot2 + "\\" + G.ImageFileName);
                            }
                        }
                        _unitsqlitemrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });
                        var olditem = _sqlitemrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        foreach (var items in olditem)
                        {
                            _sqlitemrepository.Delete(items);
                            _seosqlrepository.DelDataUseWhere("TypeName='ArticleItem' and TypeID=@1", new object[] { items.ItemID });
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
        public Paging<GroupArticle> PagingGroup(SearchModelBase model)
        {
            var Paging = new Paging<GroupArticle>();
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

        #region PagingSubGroup
        public Paging<GroupItemArticle> PagingSubGroup(SearchModelBase model)
        {
            var Paging = new Paging<GroupItemArticle>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("Group_ID=" + model.ModelID);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("Group_Name like @1");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _groupsubsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);

            Paging.total = _groupsubsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region EditGroup
        public string EditGroup(string name, string id, string mainid, string account,string uploadfilename,string uploadfileorgname, bool hassubgroup)
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
                var group = new GroupArticle();
                group.Group_Name = name;
                group.Enabled = true;
                group.Readonly = false;
                group.Seo_Manage = false;
                group.Main_ID = int.Parse(mainid);
                group.Sort = maxsort+1;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                group.HasSubGroup = hassubgroup;
                group.ImageFileName = uploadfilename;
                group.ImageFileOrgName = uploadfileorgname;
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
                var checkdata = _groupsqlrepository.GetByWhere("Group_Name=@1 and ID!=@2 AND Main_ID=@3", new object[] { name, id, mainid });
                if (checkdata.Count() > 0)
                {
                    return "類別名稱已經存在";
                }

                var olddata= _groupsqlrepository.GetByWhere("ID=@1", new object[] {  id });
                var group = new GroupArticle();
                group.ID = int.Parse(id);
                group.Group_Name = name;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                group.HasSubGroup = hassubgroup;
                if (hassubgroup == false) {
                    _articelgroupsqlrepository.DelDataUseWhere("GroupID=@1 and SubGroupID>-1", new object[] { group.ID });
                    _groupsubsqlrepository.DelDataUseWhere("GroupID=@1", new object[] { group.ID });
                }
                if (uploadfilename != "") {
                    group.ImageFileName = uploadfilename;
                    group.ImageFileOrgName = uploadfileorgname;
                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\Article\\";
                    if (System.IO.File.Exists(oldroot + "\\" + olddata.First().ImageFileName))
                    {
                        System.IO.File.Delete(oldroot + "\\" + olddata.First().ImageFileName);
                    }

                }
                r = _groupsqlrepository.Update(group);
                if (r > 0)
                {
                    var oldfilename = olddata.First().ImageFileName;
      
                    return "修改成功";
                }
                else
                {
                    return "修改失敗";
                }

            }
        }
        #endregion

        #region EditSubGroup
        public string EditSubGroup(string name, string id, string mainid, string account, string uploadfilename, string uploadfileorgname,string groupid)
        {
            var r = 0;
            if (id == "-1" || id.IsNullorEmpty())
            {
                var alldata = _groupsubsqlrepository.GetByWhere("Group_ID=@1", new object[] { groupid });
                if (alldata.Any(v => v.Group_Name == name))
                {
                    return "類別名稱已經存在";
                }

                var maxsort = _groupsubsqlrepository.GetDataCaculate("Max(Sort)", "Group_ID=@1", new object[] { groupid });
                var group = new GroupItemArticle();
                group.Group_Name = name;
                group.Enabled = true;
                group.Readonly = false;
                group.Seo_Manage = false;
                group.Main_ID = int.Parse(mainid);
                group.Sort = maxsort + 1;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                group.ImageFileName = uploadfilename;
                group.ImageFileOrgName = uploadfileorgname;
                group.Group_ID = int.Parse(groupid);
                r = _groupsubsqlrepository.Create(group);
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
                var checkdata = _groupsubsqlrepository.GetByWhere("Group_Name=@1 and ID!=@2 AND Main_ID=@3", new object[] { name, id, mainid });
                if (checkdata.Count() > 0)
                {
                    return "類別名稱已經存在";
                }

                var olddata = _groupsubsqlrepository.GetByWhere("ID=@1", new object[] { id });
                var group = new GroupItemArticle();
                group.ID = int.Parse(id);
                group.Group_Name = name;
                group.UpdateDatetime = DateTime.Now;
                group.UpdateUser = account;
                group.Group_ID = int.Parse(groupid);
                if (uploadfilename != "")
                {
                    group.ImageFileName = uploadfilename;
                    group.ImageFileOrgName = uploadfileorgname;
                    var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\Article\\";
                    if (System.IO.File.Exists(oldroot + "\\" + olddata.First().ImageFileName))
                    {
                        System.IO.File.Delete(oldroot + "\\" + olddata.First().ImageFileName);
                    }

                }
                r = _groupsubsqlrepository.Update(group);
                if (r > 0)
                {
                    var oldfilename = olddata.First().ImageFileName;

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
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\Article\\";
             
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var users = _sqlitemrepository.GetByWhere("GroupID=@1", new object[] { idlist[idx] });
                    if (users.Count() > 0)
                    {
                        var gname = _groupsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().Group_Name;
                        return "群組名稱:" + gname + " 目前已被使用,無法刪除";
                    }
                    var fname = _groupsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().ImageFileName;
                    var entity = new GroupArticle();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _groupsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除群組失敗:ID=" + idlist[idx]);
                    }
                    if (System.IO.File.Exists(oldroot + "\\" + fname))
                    {
                        System.IO.File.Delete(oldroot + "\\" + fname);
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
                var alldata = _groupsqlrepository.GetAll().OrderBy(v => v.Sort).ToArray();
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
                var entity = new GroupArticle();
                entity.Enabled = status ? true : false;
                entity.UpdateDatetime = DateTime.Now;
                entity.ID = int.Parse(id);
                entity.UpdateUser = account;
                var r = _groupsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改GroupArticle顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改GroupArticle顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateGroupSubStatus
        public string UpdateGroupSubStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new GroupItemArticle();
                entity.Enabled = status ? true : false;
                entity.UpdateDatetime = DateTime.Now;
                entity.ID = int.Parse(id);
                entity.UpdateUser = account;
                var r = _groupsubsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改GroupItemArticle顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改GroupItemArticle顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region GetUnitModel
        public ArticleUnitSettingModel GetUnitModel(string modelid)
        {
            var data = _unitsqlitemrepository.GetByWhere("MainID=@1", new object[] { modelid });
            var model = new ArticleUnitSettingModel();
            model.MainID = int.Parse(modelid);
            if (data.Count() > 0)
            {
                model = new ArticleUnitSettingModel()
                {
                    ClassOverview = data.First().ClassOverview,
                    Column1 = data.First().Column1,
                    Column2 = data.First().Column2,
                    Column3 = data.First().Column3,
                    Column4 = data.First().Column4,
                    Column5 = data.First().Column5,
                    Column6 = data.First().Column6,
                    Column7 = data.First().Column7,
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
                GeneralStudentAuth = data.First().GeneralStudentAuth,
                 IntroductionHtml = data.First().IntroductionHtml,
                    Summary = data.First().Summary.IsNullorEmpty() ? "" : data.First().Summary,
                    SummaryIn = data.First().SummaryIn.IsNullorEmpty() ? "" : data.First().SummaryIn
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
            if (model.UnitSettingColumnList.Count() == 0) {
                var columnlist = _columnsqlrepository.GetByWhere("Type='Article'", null).OrderBy(v=>v.Sort);
                foreach (var c in columnlist) {
                    model.UnitSettingColumnList.Add(new UnitSettingColumn()
                    {
                        Name = c.ColumnName,
                        Sellected = 1
                    });
                }
            }
            model.ColumnNameMapping = new Dictionary<string, string>();
            model.ColumnNameMapping.Add("序號", model.Column1.IsNullorEmpty() ? "序號" : model.Column1);
            model.ColumnNameMapping.Add("發佈日期", model.Column2.IsNullorEmpty() ? "發佈日期" : model.Column2);
            model.ColumnNameMapping.Add("標題", model.Column3.IsNullorEmpty() ? "標題" : model.Column3);
            model.ColumnNameMapping.Add("相關連結", model.Column4.IsNullorEmpty() ? "相關連結" : model.Column4);
            model.ColumnNameMapping.Add("檔案下載", model.Column5.IsNullorEmpty() ? "檔案下載" : model.Column5);
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
            wherestr.Add("Type='Article'");
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
                    mtseqdata = _columnsqlrepository.GetByWhere("Sort>@1 and Type=@2", new object[] { seq, "Article" }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _columnsqlrepository.GetCountUseWhere("Type=@1", new object[] { "Article" });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ColumnSetting> ltseqdata = null;
                    ltseqdata = _columnsqlrepository.GetByWhere("Sort<=@1  and Type=@2", new object[] { qseq, "Article" }).OrderBy(v => v.Sort).ToArray();
                    //更新seq+1
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
        public string SetUnitModel(ArticleUnitSettingModel model, string account)
        {
            var newmodel = new ArticleUnitSetting();
            newmodel.UpdateDatetime = DateTime.Now;
            newmodel.UpdateUser = account;
            var columnsetting = "";
            if (model.ColumnName != null) {
                columnsetting=    string.Join(",", model.ColumnName) + "@" + string.Join(",", model.ColumnUse);
            }
         
            var r = 0;
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
                newmodel.IntroductionHtml = model.IntroductionHtml == null ? "" : model.IntroductionHtml;
                newmodel.Summary = model.Summary == null ? "" : model.Summary;
                newmodel.SummaryIn = model.SummaryIn == null ? "" : model.SummaryIn;
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
                newmodel.IntroductionHtml = model.IntroductionHtml == null ? "" : model.IntroductionHtml;
                newmodel.Summary = model.Summary == null ? "" : model.Summary;
                newmodel.SummaryIn = model.SummaryIn == null ? "" : model.SummaryIn;
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

        #region GetSEO
        public SEOViewModel GetSEO(string modelid)
        {
            var seodata = _seosqlrepository.GetByWhere("TypeName=@1 and TypeID=@2", new object[] { "Article", modelid });
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
                model.TypeID = modelid;
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
                Title = model.WebsiteTitle==null?"" : model.WebsiteTitle,
                TypeName = "Article",
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

        #region CreateItem
        public string CreateItem(ArticleEditModel model, string LangId, string account)
        {
            var iswriteseo = false;
            if (model.WebsiteTitle.IsNullorEmpty() == false || model.Description.IsNullorEmpty() == false
                 || (model.Keywords != null && model.Keywords.Any(v => v.IsNullorEmpty() == false)))
            {
                iswriteseo = true;
            }
            //var accountdata=
            var olddata = _sqlitemrepository.GetByWhere("ModelID=@1", new object[] { model.ModelID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new ArticleItem()
            {
                HtmlContent = model.HtmlContent,
                ImageFileDesc = model.ImageFileDesc,
                ImageFileLocation = model.ImageFileLocation,
                ModelID = model.ModelID,
                ImageFileOrgName = model.ImageFileOrgName,
                LinkUrl = model.LinkUrl,
                UploadFileDesc = model.UploadFileDesc,
                UploadFileName = model.UploadFileName,
                UploadFilePath = model.UploadFilePath,
                ImageFileName = model.ImageFileName,
                Sort = 1,
                ClickCnt = 0,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(LangId),
                Title = model.Title,
                CreateDatetime = DateTime.Now,
                CreateUser = account,
                Enabled = true,
                Link_Mode =1,
                RelateImageFileName = model.RelateImageName,
                RelateImageFileOrgName = model.RelateImageFileOrgName,
                Introduction = model.Introduction == null ? "" : model.Introduction,
                CreateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = true,
                LinkUrlDesc = model.LinkUrlDesc,
                IsShowMoreArticle = model.IsShowMoreArticle,
                ArticleLink = model.ArticleLink == null ? "" : model.ArticleLink,
                IsNew = model.IsNew,
                ExtItemList=model.ExtItemList==null?"": model.ExtItemList
            };
            if (model.ActiveID.IsNullorEmpty() == false)
            {
                savemodel.ActiveID = int.Parse(model.ActiveID);
            }
            if (model.ActiveItemID.IsNullorEmpty() == false)
            {
                savemodel.ActiveItemID = int.Parse(model.ActiveItemID);
            }
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
            var r = _sqlitemrepository.Create(savemodel);
            if (r > 0)
            {
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                if (string.IsNullOrEmpty(model.GroupItemList) == false)
                {
                    _articelgroupsqlrepository.DelDataUseWhere("ItemID=@1", new object[] { savemodel.ItemID });
                    var iid = model.GroupItemList.Split(',');
                    var grouplist = _groupsqlrepository.GetAll().ToArray();
                    var groupsublist = _groupsubsqlrepository.GetAll().ToArray();
                    foreach (var i in iid)
                    {
                        var gid = "";
                        var subgid = "-1";
                        if (i.IndexOf("mtd_") >= 0)
                        {
                            _articelgroupsqlrepository.Create(new ArticelGroupItem()
                            {
                                 GroupID=int.Parse(i.Replace("mtd_", "")),
                                  ItemID= savemodel.ItemID,
                                   MainID= savemodel.ModelID.Value,
                                  SubGroupID=-1
                            });
                        }
                        else {
                            var nowid = i.Replace("td_", "");
                            var item = groupsublist.Where(v => v.ID.ToString() == nowid);
                            _articelgroupsqlrepository.Create(new ArticelGroupItem()
                            {
                                GroupID = item.First().Group_ID.Value,
                                ItemID = savemodel.ItemID,
                                MainID = savemodel.ModelID.Value,
                                SubGroupID = item.First().ID.Value
                            });
                        }
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
                        TypeName = "ArticleItem",
                        TypeID = savemodel.ItemID,
                         Lang_ID= int.Parse(LangId)
                    });
                }
                foreach (var odata in olddata)
                {
                    _sqlitemrepository.Update("Sort=@1", "ItemID=@2", new object[] { odata.Sort + 1, odata.ItemID });
                }
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }

        }
        #endregion

        #region GetModelByID
        public ArticleEditModel GetModelByID(string modelid, string itemid)
        {
            var data = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { modelid });
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var seodata = _seosqlrepository.GetByWhere("TypeName='ArticleItem' and TypeID=@1", new object[] { itemid });
                var model = new ArticleEditModel()
                {
                    ItemID = fdata.ItemID,
                    ModelName = maindata.Count() == 0 ? "" : maindata.First().Name,
                    Description = seodata.Count() > 0 ? seodata.First().Description : "",
                    ImageFileName = fdata.ImageFileName,
                    ImageFileOrgName = fdata.ImageFileOrgName,
                    UploadFileDesc = fdata.UploadFileDesc,
                    UploadFileName = fdata.UploadFileName,
                    UploadFilePath = fdata.UploadFilePath,
                    WebsiteTitle = seodata.Count() > 0 ? seodata.First().Title : "",
                    Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10},
                    HtmlContent = fdata.HtmlContent,
                    ImageFileDesc = fdata.ImageFileDesc,
                    ImageFileLocation = fdata.ImageFileLocation,
                    LinkUrl = fdata.LinkUrl,
                    ModelID = fdata.ModelID.Value,
                    ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/ArticleItem/" + fdata.ImageFileName),
                    ActiveID = fdata.ActiveID == null ? "" : fdata.ActiveID.ToString(),
                    ActiveItemID = fdata.ActiveItemID == null ? "" : fdata.ActiveItemID.ToString(),
                    EdDate = fdata.EdDate,
                    EdDateStr = fdata.EdDate == null ? "" : fdata.EdDate.Value.ToString("yyyy/MM/dd"),
                    Group_ID = fdata.GroupID == null ? -1 : fdata.GroupID.Value,
                    Link_Mode = fdata.Link_Mode,
                    PublicshStr = fdata.PublicshDate == null ? "" : fdata.PublicshDate.Value.ToString("yyyy/MM/dd"),
                    StDate = fdata.StDate,
                    StDateStr = fdata.StDate == null ? "" : fdata.StDate.Value.ToString("yyyy/MM/dd"),
                    Title = fdata.Title,
                     RelateImageFileOrgName = fdata.RelateImageFileOrgName,
                      RelateImageName = fdata.RelateImageFileName,
                        RelateImagelUrl= VirtualPathUtility.ToAbsolute("~/UploadImage/ArticleItem/" + fdata.RelateImageFileName),
                        Introduction= fdata.Introduction,
                         CreateDatetime= fdata.CreateDatetime==null?"": fdata.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                         CreateUser= fdata.CreateName,
                         UpdateDatetime = fdata.UpdateDatetime == null ? "" : fdata.UpdateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                     UpdateUser= fdata.UpdateName,
                      LinkUrlDesc=fdata.LinkUrlDesc,
                    ArticleLink = fdata.ArticleLink,
                    IsShowMoreArticle = fdata.IsShowMoreArticle.Value,
                    IsNew = fdata.IsNew == null ? false : fdata.IsNew.Value,
                    EdDateNewStr = fdata.EdDateNew == null ? "" : fdata.EdDateNew.Value.ToString("yyyy/MM/dd"),
                    StDateNewStr = fdata.StDateNew == null ? "" : fdata.StDateNew.Value.ToString("yyyy/MM/dd"),
                     ExtItemList=fdata.ExtItemList==null?"": fdata.ExtItemList
                };
                var gitem = _articelgroupsqlrepository.GetByWhere("ItemID=@1 order by GroupID,SubGroupID", new object[] { itemid });
                var itemlist = new List<string>();
                var listr = "";
                var grouplist = _groupsqlrepository.GetAll().ToArray();
                var groupsublist = _groupsubsqlrepository.GetAll().ToArray();
                foreach (var item in gitem) {
                    if (item.SubGroupID > 0)
                    {
                        var _g = grouplist.Where(v => v.ID == item.GroupID);
                        var _gsub = groupsublist.Where(v => v.ID == item.SubGroupID);
                        listr += "<li>" + _g.First().Group_Name + "-" + _gsub.First().Group_Name + "</li>";
                        itemlist.Add("td_" + item.SubGroupID);
                    }
                    else {
                        itemlist.Add("mtd_" + item.GroupID);
                        var _g= grouplist.Where(v => v.ID == item.GroupID);
                        listr += "<li>" + _g.First().Group_Name + "</li>";
                    }
                }
                model.GroupNameListStr = listr;
                model.GroupItemList = string.Join(",", itemlist.ToArray());
                var extlist = new List<ExtItemObj>();
                if (model.ExtItemList != "") {
                    var alllistname = _sqlitemrepository.GetByWhere("ModelID=@1", new object[] { model.ModelID });
                    var extsp = model.ExtItemList.Split(',');
                    foreach (var ex in extsp) {
                        var item = alllistname.Where(v => v.ItemID.ToString() == ex);
                        if (item.Count() > 0) {
                            var nameli = "<li>" + item.First().Title + "</li>";
                            model.ExtLiList += nameli;
                            extlist.Add(new ExtItemObj()
                            {
                                id = ex,
                                name = nameli
                            });
                        }
                    }
                }
                model.ExtItemObj = Newtonsoft.Json.JsonConvert.SerializeObject(extlist);
                return model;
            }
            else
            {
                ArticleEditModel model = new ArticleEditModel();
                model.Keywords = new string[10];
                model.ModelID = int.Parse(modelid);
                model.ImageFileLocation = "1";
                model.ModelName = maindata.Count() == 0 ? "" : maindata.First().Name;
                model.GroupItemList = "";
                model.ExtItemObj = "[]";
                return model;
            }

        }
        #endregion

        #region UpdateItem
        public string UpdateItem(ArticleEditModel model, string LangId, string account)
        {
            var iswriteseo = false;
            if (model.WebsiteTitle.IsNullorEmpty() == false || model.Description.IsNullorEmpty() == false
                   || (model.Keywords!=null && model.Keywords.Any(v => v.IsNullorEmpty() == false)))
            {
                iswriteseo = true;
            }
            var olddata = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ArticleItem\\";
            //刪除舊檔案
            if (model.UploadFileName.IsNullorEmpty())
            {
                if (System.IO.File.Exists(olddata.First().UploadFilePath))
                {
                    System.IO.File.Delete(olddata.First().UploadFilePath);
                }
                model.UploadFilePath = "";
                model.UploadFileName = "";
            }
            else
            {
                if (olddata.First().UploadFileName != model.UploadFileName)
                {
                    if (System.IO.File.Exists(olddata.First().UploadFilePath))
                    {
                        System.IO.File.Delete(olddata.First().UploadFilePath);
                    }
                }
            }
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

            _seosqlrepository.DelDataUseWhere("TypeName='ArticleItem' and TypeID=@1", new object[] { model.ItemID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new ArticleItem()
            {
                ItemID= model.ItemID,
                HtmlContent = model.HtmlContent==null?"" : model.HtmlContent,
                ImageFileDesc = model.ImageFileDesc == null ? "" : model.ImageFileDesc,
                ImageFileLocation = model.ImageFileLocation == null ? "" : model.ImageFileLocation,
                RelateImageFileName = model.RelateImageName,
                RelateImageFileOrgName = model.RelateImageFileOrgName ,
                ModelID = model.ModelID,
                ImageFileOrgName = model.ImageFileOrgName ,
                LinkUrl = model.LinkUrl == null ? "" : model.LinkUrl == null ? "" : model.LinkUrl,
                LinkUrlDesc = model.LinkUrlDesc == null ? "" : model.LinkUrlDesc == null ? "" : model.LinkUrlDesc,
                UploadFileDesc = model.UploadFileDesc == null ? "" : model.UploadFileDesc,
                UploadFileName = model.UploadFileName ,
                UploadFilePath = model.UploadFilePath == null ? "" : model.UploadFilePath,
                ImageFileName = model.ImageFileName,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(LangId),
                Title = model.Title == null ? "" : model.Title,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                Enabled = true,
                Link_Mode = 1,
                Introduction= model.Introduction == null ? "" : model.Introduction,
                UpdateName    = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift=true,
                IsShowMoreArticle = model.IsShowMoreArticle,
                ArticleLink = model.ArticleLink == null?"": model.ArticleLink,
                IsNew = model.IsNew,
                ExtItemList = model.ExtItemList == null ? "" : model.ExtItemList
            };
            if (model.ActiveID.IsNullorEmpty() == false)
            {
                savemodel.ActiveID = int.Parse(model.ActiveID);
            }
            if (model.ActiveItemID.IsNullorEmpty() == false)
            {
                savemodel.ActiveItemID = int.Parse(model.ActiveItemID);
            }
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
            var r = _sqlitemrepository.Update(savemodel);
            if (r > 0)
            {
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                if (string.IsNullOrEmpty(model.GroupItemList) == false)
                {
                    _articelgroupsqlrepository.DelDataUseWhere("ItemID=@1", new object[] { savemodel.ItemID });
                    var iid = model.GroupItemList.Split(',');
                    var grouplist = _groupsqlrepository.GetAll().ToArray();
                    var groupsublist = _groupsubsqlrepository.GetAll().ToArray();
                    foreach (var i in iid)
                    {
                        if (i.IndexOf("mtd_") >= 0)
                        {
                            _articelgroupsqlrepository.Create(new ArticelGroupItem()
                            {
                                GroupID = int.Parse(i.Replace("mtd_", "")),
                                ItemID = savemodel.ItemID,
                                MainID = savemodel.ModelID.Value,
                                SubGroupID = -1
                            });
                        }
                        else
                        {
                            var nowid = i.Replace("td_", "");
                            var item = groupsublist.Where(v => v.ID.ToString() == nowid);
                            _articelgroupsqlrepository.Create(new ArticelGroupItem()
                            {
                                GroupID = item.First().Group_ID.Value,
                                ItemID = savemodel.ItemID,
                                MainID = savemodel.ModelID.Value,
                                SubGroupID = item.First().ID.Value
                            });
                        }
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
                        TypeName = "ArticleItem",
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
        public Paging<ArticleItemResult> PagingItem(string modelid, ArticleSearchModel model)
        {
            var Paging = new Paging<ArticleItemResult>();
            var data = new List<ArticleItem>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(modelid);

            if (string.IsNullOrEmpty(model.Title) == false)
            {
                idx += 1;
                wherestr.Add("(Title like @" + idx + " or HtmlContent like @"+idx+")");
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
                wherestr.Add("(PublicshDate >=@" + idx+ " or PublicshDate=Null)");
                whereobj.Add(model.PublicshDateFrom);
            }
            if (string.IsNullOrEmpty(model.PublicshDateTo) == false)
            {
                idx += 1;
                wherestr.Add("(PublicshDate <=@" + idx +" or  PublicshDate=Null)");
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
            if (string.IsNullOrEmpty(model.GroupId)==false|| string.IsNullOrEmpty(model.SubGroupId) == false)
            {
                var allgroup = _articelgroupsqlrepository.GetByWhere("MainID=@1", new object[] { modelid });
                var str = string.Join(" and ", wherestr);
                data = _sqlitemrepository.GetByWhere(str+" Order by " + model.Sort, whereobj.ToArray()).ToList();
                if (model.GroupId == "0"|| model.GroupId == "")
                {
                    data = data.Where(v => allgroup.Any(x => x.ItemID == v.ItemID)==false).ToList();
                }
                else {
                    allgroup = allgroup.Where(v => v.GroupID ==int.Parse( model.GroupId));
                    if (model.SubGroupId != null) {
                        allgroup = allgroup.Where(v => v.SubGroupID== int.Parse(model.SubGroupId));
                    }
                    data = data.Where(v => allgroup.Any(x => x.ItemID == v.ItemID)).ToList();
                }
                data=data.Skip(model.Offset).Take(model.Limit).ToList();
                Paging.total = data.Count();
            }
            else {
                if (wherestr.Count() > 0)
                {
                    var str = string.Join(" and ", wherestr);
                    data = _sqlitemrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                    Paging.total = _sqlitemrepository.GetCountUseWhere(str, whereobj.ToArray());

                }
                else
                {
                    Paging.total = _sqlitemrepository.GetAllDataCount();
                    data = _sqlitemrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
                }
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
                var gname = ALLGroup.Where(v => v.ID == d.GroupID);
                Paging.rows.Add(new ArticleItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    IsRange = isrange == true ? "是" : "否",
                    GroupName = gname.Count() > 0 ? gname.First().Group_Name : "無分類",
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy/MM/dd"),
                     Sort=d.Sort.Value
                });

            }
            return Paging;
        } 
        #endregion

        #region UpdateItemSeq
        public string UpdateItemSeq(int modelid,int id, int seq,  string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<ArticleItem> mtseqdata = null;
                    mtseqdata = _sqlitemrepository.GetByWhere("Sort>@1 and ModelID=@2", new object[] { seq, modelid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlitemrepository.GetCountUseWhere("ModelID=@1",new object[] { modelid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ArticleItem> ltseqdata = null;
                    ltseqdata = _sqlitemrepository.GetByWhere("Sort<=@1 and ModelID=@2", new object[] { qseq, modelid }).OrderBy(v => v.Sort).ToArray();
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
                        _sqlitemrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _sqlitemrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新ArticleItem排序失敗:" + " error:" + ex.Message);
                return "更新ArticleItem排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetItemStatus
        public string SetItemStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new ArticleItem();
                entity.Enabled = status ? true : false;
                entity.ItemID = int.Parse(id);
                var r = _sqlitemrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改ArticleItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改ArticleItem顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region DeleteItem
        public string DeleteItem(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\ArticleItem\\";
                var oldroot2 = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\Article\\";
                var modelid = -1;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ArticleItem();
                    var olditem = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { idlist[idx] });
                    modelid = olditem.First().ModelID.Value;
                    entity.ItemID = int.Parse(idlist[idx]);
                    r = _sqlitemrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除訊息項目失敗:ID=" + idlist[idx]);
                    }
                    else {

                        _articelgroupsqlrepository.DelDataUseWhere("ItemID=@1", new object[] { idlist[idx] });
                        _seosqlrepository.DelDataUseWhere("TypeName='ArticleItem' and TypeID=@1", new object[] { idlist[idx] });
                        if (olditem.First().UploadFileName.IsNullorEmpty()==false)
                        {
                            if (System.IO.File.Exists(olditem.First().UploadFilePath))
                            {
                                System.IO.File.Delete(olditem.First().UploadFilePath);
                            }
                        }
                        if (olditem.First().ImageFileName.IsNullorEmpty()==false)
                        {

                            if (System.IO.File.Exists(oldroot + "\\" + olditem.First().ImageFileName))
                            {
                                System.IO.File.Delete(oldroot + "\\" + olditem.First().ImageFileName);
                            }
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除訊息項目:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }
                var alldata = _sqlitemrepository.GetByWhere("ModelId=@1", new object[] { modelid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    _sqlitemrepository.Update("Sort=@1","ItemID=@2",new object[] { idx , tempmodel .ItemID});
                }
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除群組失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region PagingItemForWebSite
        public Paging<ArticleItemResult> PagingItemForWebSite(string modelid, ArticleSearchModel model,string nogroupstr)
        {
            var Paging = new Paging<ArticleItemResult>();
            var data = new List<ArticleItem>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(modelid);

            if (string.IsNullOrEmpty(model.Title) == false)
            {
                idx += 1;
                wherestr.Add("(Title like @" + idx + " or HtmlContent like @" + idx + ")");
                whereobj.Add("%" + model.Title + "%");
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
            if (string.IsNullOrEmpty(model.GroupId) == false || string.IsNullOrEmpty(model.SubGroupId) == false)
            {
                var allgroup = _articelgroupsqlrepository.GetByWhere("MainID=@1", new object[] { modelid });
                var str = string.Join(" and ", wherestr);
                data = _sqlitemrepository.GetByWhere(str + " Order by " + model.Sort, whereobj.ToArray()).ToList();
                if (model.GroupId == "0" || model.GroupId == "")
                {
                    data = data.Where(v => allgroup.Any(x => x.ItemID == v.ItemID) == false).ToList();
                }
                else
                {
                    allgroup = allgroup.Where(v => v.GroupID == int.Parse(model.GroupId));
                    if (model.SubGroupId .IsNullorEmpty()==false)
                    {
                        allgroup = allgroup.Where(v => v.SubGroupID == int.Parse(model.SubGroupId));
                    }
                    data = data.Where(v => allgroup.Any(x => x.ItemID == v.ItemID)).ToList();
                }
                data = data.Skip(model.Offset).Take(model.Limit).ToList();
                Paging.total = data.Count();
            }
            else
            {
                if (wherestr.Count() > 0)
                {
                    var str = string.Join(" and ", wherestr);
                    data = _sqlitemrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                    Paging.total = _sqlitemrepository.GetCountUseWhere(str, whereobj.ToArray());

                }
                else
                {
                    Paging.total = _sqlitemrepository.GetAllDataCount();
                    data = _sqlitemrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
                }
            }
            var sidx = model.Offset;
            foreach (var d in data)
            {
                var isnew = false;
                if (d.IsNew.Value)
                {
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
                Paging.rows.Add(new ArticleItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy.MM.dd"),
                    Sort = sidx,
                     LinkUrl=d.LinkUrl==null?"": d.LinkUrl,
                     UploadFileName= d.UploadFileName == null ? "" : d.UploadFileName,
                    Link_Mode=d.Link_Mode,
                    Introduction=d.Introduction==null?"":d.Introduction,
                    RelatceImageFileName= d.RelateImageFileName == null ? "" : d.RelateImageFileName,
                    IsNew= isnew,
                    LinkUrlDesc = d.LinkUrlDesc == null ? "" : d.LinkUrlDesc,
                    FileUrlDesc = d.UploadFileDesc == null ? "" : d.UploadFileDesc,
                });

            }
            return Paging;
        }
        #endregion

        #region GetModelItem
        public ArticleItem GetModelItem(string itemid)
        {
            var data = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            if (data.Count() > 0)
            {
                return data.First();
            }
            else
            {
                return new ArticleItem();
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
            _clicktablesqlitemrepository.DelDataUseWhere("Type='Article' and LastClickDatetime<@1", new object[] { DateTime.Now.AddDays(-1) });
            var hasdata = _clicktablesqlitemrepository.GetByWhere("Type='Article' and IP=@1 and ID=@2", new object[] { tClientIP, itemid });
            if (hasdata.Count() == 0)
            {
                _sqlitemrepository.Update("ClickCnt=ClickCnt+1", "ItemID=@1", new object[] { itemid });
                _clicktablesqlitemrepository.Create(new ClickCountTable()
                {
                    IP = tClientIP,
                    LastClickDatetime = DateTime.Now,
                    Type = "Article",
                    ID = itemid
                });
            }
            
        }
        #endregion

        #region GetSyndicationFeedData
        public SyndicationFeed GetSyndicationFeedData(string itemid,string menuid)
        {
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var mainmodel = _sqlrepository.GetByWhere("ID=@1", new object[] { itemid });
            var hostUrl = string.Format("{0}://{1}",
                  System.Web.HttpContext.Current.Request.Url.Scheme,
                  System.Web.HttpContext.Current.Request.Url.Authority);
            var feed = new SyndicationFeed(
              "MySkylines",
              mainmodel.First().Name + " - Rss Feed",
              new Uri(HttpContext.Current.Request.Url.AbsoluteUri));
            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("ModelID=@1");
            whereobj.Add(itemid);
            var nowdate = DateTime.Now.ToString("yyyy/MM/dd");
            idx += 1;
            wherestr.Add("PublicshDate <=@" + idx);
            whereobj.Add(nowdate);
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
            var  data = _sqlitemrepository.GetDataUseWhereRange(str, whereobj.ToArray(), 0,100, "Sort Desc").ToList();
            List<SyndicationItem> items = new List<SyndicationItem>();
            foreach (var d in data)
            {
                var content = d.HtmlContent == null ? "" : d.HtmlContent.TrimgHtmlTag();
                if (content.Length >= 100) { content = content.Substring(0, 100); }
                var url = "";
                if (menuid.IsNullorEmpty())
                {
                    url = helper.Action("ArticleView", new { id= d.ItemID });
                }
                else {
                    url = helper.Action("ArticleView", new { id = d.ItemID, mid = menuid });
                }
                SyndicationItem item = new SyndicationItem(
                d.Title.TrimgHtmlTag(), content,new Uri(hostUrl + url),d.ItemID.ToString(),
                d.PublicshDate.Value);
                if (d.PublicshDate!=null)
                {
                    item.PublishDate = d.PublicshDate.Value;
                }
                else {
                    item.PublishDate = DateTime.Now;
                }
                items.Add(item);
            }
            feed.Items = items;
            return feed;
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


        public string[] GetArticleMore(string mainID,string nowid,string menuid)
        {
            var idx = 1;
            var whereobj = new List<object>();
            var wherestr = new List<string>();
            wherestr.Add("ModelID=@1");
            whereobj.Add(mainID);
            var nowdate = DateTime.Now.ToString("yyyy/MM/dd");

            idx += 1;
            wherestr.Add("ItemID<>@" + idx);
            whereobj.Add(nowid);

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
            var items = _sqlitemrepository.GetByWhere(str + " order by Sort", whereobj.ToArray()).ToList();
            var sb = new System.Text.StringBuilder();
        
            UrlHelper helper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            var baseimg = helper.Content("~/ContentWebsite/image/logo_400x300.jpg");
            foreach (var v in items) {
                sb.Append("<div class='item'>");
                if (v.Link_Mode == 1)
                {
                    if (menuid != "-1")
                    {
                        sb.Append("<a href='" + helper.Action("ArticleView", new { id = v.ItemID, mid = menuid,  groupid = v.GroupID }) + "' title='" + v.Title + "'>");
                    }
                    else
                    {
                        sb.Append("<a href='" + helper.Action("ArticleView", new { id = v.ItemID,groupid = v.GroupID }) + "' title='" + v.Title + "'> ");
                    }
                }
                else
                {
                    sb.Append("<a href='#' style='cursor: default;pointer-events: none' title='" + v.Title +"'>");
                }
                if (v.RelateImageFileName != "")
                {
                    sb.Append("<img src = '" + helper.Content("~/UploadImage/ArticleItem/" + v.RelateImageFileName) + "'  alt='" + v.Title+"' align='left'>");
                }
                else
                {
                    sb.Append("<img src = '" + baseimg + "' alt='" + v.Title+"'>");
                }
                sb.Append("<div class='con_font'>"+v.Title+"</div></a></div>");       
            }
            items = items.Take(4).ToList();
            var sb2 = new System.Text.StringBuilder();
            foreach (var v in items)
            {
                sb2.Append("<div class='item'>");
                if (v.Link_Mode == 1)
                {
                    if (menuid != "-1")
                    {
                        sb2.Append("<a href='" + helper.Action("ArticleView", new { id = v.ItemID, mid = menuid, groupid = v.GroupID }) + "' title='" + v.Title + "'>");
                    }
                    else
                    {
                        sb2.Append("<a href='" + helper.Action("ArticleView", new { id = v.ItemID, groupid = v.GroupID }) + "' title='" + v.Title + "'> ");
                    }
                }
                else
                {
                    sb2.Append("<a href='#' style='cursor: default;pointer-events: none' title='" + v.Title +"'>");
                }
                if (v.RelateImageFileName != "")
                {
                    sb2.Append("<img src = '" + helper.Content("~/UploadImage/ArticleItem/" + v.RelateImageFileName) + "'  alt='" + v.Title+"' align='left'>");
                }
                else
                {
                    sb2.Append("<img src = '" + baseimg + "' alt='" + v.Title+"'>");
                }
                sb2.Append("<div class='con_font'>" + v.Title + "</div></a></div>");
            }

            return new string[] { sb.ToString(), sb2.ToString() };
        }

        #region GetAllGroupList
        public List<GroupArticle> GetAllGroupList(string mainid)
        {
            var list = _groupsqlrepository.GetByWhere("Enabled=@1 and Main_ID=@2 order by sort", new object[] { 1, mainid }).ToList();
            return list;
        }

        #endregion

        #region GetExtItemObj
        public List<ExtItemObj> GetExtItemObj(string mainid,string ExtItemList)
        {
            var extlist = new List<ExtItemObj>();
            if (ExtItemList.IsNullorEmpty()==false)
            {
                var alllistname = _sqlitemrepository.GetByWhere("ModelID=@1", new object[] { mainid });
                var extsp = ExtItemList.Split(',');

                foreach (var ex in extsp)
                {
                    var item = alllistname.Where(v => v.ItemID.ToString() == ex);
                    if (item.Count() > 0)
                    {
                        var nameli = item.First().Title;
                        extlist.Add(new ExtItemObj()
                        {
                            id = ex,
                            name = nameli
                        });
                    }
                }
            }
            return extlist;
        }
        #endregion

        #region GetAllGroupList
        public List<GroupItemArticle> GetAllGroupSubList(string mainid)
        {
            var list = _groupsubsqlrepository.GetByWhere("Enabled=@1 and Main_ID=@2 order by sort", new object[] { 1, mainid }).ToList();
            return list;
        }

        #endregion

        #region GetNameList
        public List<string> GetNameList(string itemid)
        {
            var grouplist = _groupsqlrepository.GetAll().ToArray();
            var groupsublist = _groupsubsqlrepository.GetAll().ToArray();
            var gitem = _articelgroupsqlrepository.GetByWhere("ItemID=@1 order by GroupID,SubGroupID", new object[] { itemid });
            var list = new List<string>();
            foreach (var item in gitem)
            {
                if (item.SubGroupID > 0)
                {
                    var _g = grouplist.Where(v => v.ID == item.GroupID);
                    var _gsub = groupsublist.Where(v => v.ID == item.SubGroupID);
                    list.Add(_g.First().Group_Name + "-" + _gsub.First().Group_Name);

                }
                else
                {
                    var _g = grouplist.Where(v => v.ID == item.GroupID);
                    var _gsub = gitem.Where(v => v.GroupID == item.GroupID && v.SubGroupID!=-1);
                    if (_gsub.Count() == 0) { list.Add(_g.First().Group_Name); }
          
                }
            }
            return list;
        }
        #endregion

        #region GetGroupByID
        public GroupArticle GetGroupByID(string groupid)
        {
            var list = _groupsqlrepository.GetByWhere("ID=@1", new object[] { groupid }).ToList();
                if (list.Count() == 0) { return new GroupArticle(); } else {
                return list.First(); 
            }
        }

        #endregion

        #region GetSubGroupList
        public List<GroupItemArticle> GetSubGroupList(string groupid)
        {
            return _groupsubsqlrepository.GetByWhere("Enabled=@1 and Group_ID=@2 order by sort", new object[] { 1, groupid }).ToList();
        }

        #endregion

        #region GetSubGroupByID
        public List<GroupItemArticle> GetSubGroupByID(string subgroupid)
        {
            return _groupsubsqlrepository.GetByWhere("ID=@1 order by sort", new object[] { subgroupid }).ToList();
        }

        #endregion
    }
}
