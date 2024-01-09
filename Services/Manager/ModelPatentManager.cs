using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using SQLModel.Models;
using SQLModel;
using ViewModels;
using Utilities;
using ViewModel;
using System.Web;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HPSF;
using System.Net.Mail;
using System.Threading;
using System.Text;
using ViewModels.DBModels;
using System.Web.Mvc;

namespace Services.Manager
{
    public class ModelPatentManager : IModelPatentManager
    {
        readonly SQLRepository<ModelPatentMain> _sqlrepository;
        readonly SQLRepository<PatentItem> _itemsqlrepository;
        readonly SQLRepository<PatentUnitSetting> _unitsqlitemrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        readonly SQLRepository<VerifyData> _verifydatasqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<GroupPatent> _groupsqlrepository;
        readonly SQLRepository<PatentDetail> _patentdetailsqlrepository;
        readonly SQLRepository<ColumnSetting> _columnsqlrepository;
        public ModelPatentManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelPatentMain;
            _Usersqlrepository= sqlinstance.Users;
            _verifydatasqlrepository = sqlinstance.VerifyData;
            _menusqlrepository = sqlinstance.Menu;
            _unitsqlitemrepository = sqlinstance.PatentUnitSetting;
            _groupsqlrepository = sqlinstance.GroupPatent;
            _itemsqlrepository = sqlinstance.PatentItem;
            _patentdetailsqlrepository = sqlinstance.PatentDetail;
            _columnsqlrepository = sqlinstance.ColumnSetting;
        }

        #region AddUnit
        public string AddUnit(string name, string langid, string account, ref int newid)
        {
            var alldata = _sqlrepository.GetByWhere("Lang_ID=@1 Order By Sort", new object[] { langid });
            var idx = 2;
            foreach (var mdata in alldata)
            {
                mdata.Sort = idx;
                _sqlrepository.Update("Sort=@1", "ID=@2", new object[] { mdata.Sort, mdata.ID });
                idx += 1;
            }
            var Model = new ModelPatentMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 19;
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

        #region GetAll
        public IEnumerable<ModelPatentMain> GetAll()
        {
            return _sqlrepository.GetAll();
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
                    var m = _menusqlrepository.GetByWhere("ModelID=19 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                    var p = _sqlrepository.GetByWhere("ModelID=19 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (p.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PatentItem\\";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelPatentMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除ModelFileDownloadMain單元失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        _verifydatasqlrepository.DelDataUseWhere("ModelID=19 and ModelMainID=@1", new object[] { idlist[idx] });
                        var allitems = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        _unitsqlitemrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });
                        _patentdetailsqlrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });
                        var olditem = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        foreach (var items in olditem)
                        {
                            _itemsqlrepository.Delete(items);
                            if (items.UploadFileName.IsNullorEmpty() == false)
                            {
                                if (System.IO.File.Exists(items.UploadFilePath))
                                {
                                    System.IO.File.Delete(items.UploadFilePath);
                                }
                            }
                        }
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除ModelPatentMain單元失敗:" + delaccount);
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
                NLogManagement.SystemLogInfo("刪除ModelPatentMain單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region Paging
        public Paging<ModelPatentMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelPatentMain>();
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
                    IList<ModelPatentMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and Lang_ID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelPatentMain> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1   and Lang_ID=@2", new object[] { qseq, langid }).OrderBy(v => v.Sort).ToArray();
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
                NLogManagement.SystemLogInfo("更新ModelPatentMain排序失敗:" + " error:" + ex.Message);
                return "更新ModelPatentMain排序失敗:" + " error:" + ex.Message;
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

        #region Where
        public IEnumerable<ModelPatentMain> Where(ModelPatentMain model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region GetGroupSelectList
        public IList<SelectListItem> GetGroupSelectList(string mainid, bool enabled)
        {
            var list = _groupsqlrepository.GetByWhere("Main_ID=@1 Order By Sort", new object[] { mainid });
            if (enabled)
            {
                list = list.Where(v => v.Enabled == true);
            }
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
        public Paging<GroupPatent> PagingGroup(SearchModelBase model)
        {
            var Paging = new Paging<GroupPatent>();
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
                var group = new GroupPatent();
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
                var checkdata = _groupsqlrepository.GetByWhere("Group_Name=@1 and ID!=@2 AND Main_ID=@3", new object[] { name, id, mainid });
                if (checkdata.Count() > 0)
                {
                    return "類別名稱已經存在";
                }
                var group = new GroupPatent();
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
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var users = _itemsqlrepository.GetByWhere("GroupID=@1", new object[] { idlist[idx] });
                    if (users.Count() > 0)
                    {
                        var gname = _groupsqlrepository.GetByWhere("ID=@1", new object[] { idlist[idx] }).First().Group_Name;
                        return "群組名稱:" + gname + " 目前已被使用,無法刪除";
                    }
                    var entity = new GroupPatent();
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
                var entity = new GroupPatent();
                entity.Enabled = status ? true : false;
                entity.UpdateDatetime = DateTime.Now;
                entity.ID = int.Parse(id);
                entity.UpdateUser = account;
                var r = _groupsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改GroupPatent顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改GroupPatent顯示狀態失敗:" + ex.Message);
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
                    IList<GroupPatent> mtseqdata = null;
                    mtseqdata = _groupsqlrepository.GetByWhere("Sort>@1 and Main_ID=@2", new object[] { seq, mainid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _groupsqlrepository.GetCountUseWhere("Main_ID=@1", new object[] { mainid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<GroupPatent> ltseqdata = null;
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

        #region PagingItem
        public Paging<PatentItemResult> PagingItem(string modelid, PatentSearchModel model)
        {
            var Paging = new Paging<PatentItemResult>();
            var data = new List<PatentItem>();

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

            if (wherestr.Count() > 0)
            {
                var str = string.Join(" and ", wherestr);
                data = _itemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _itemsqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                Paging.total = _itemsqlrepository.GetAllDataCount();
                data = _itemsqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
            }

            var ALLGroup = _groupsqlrepository.GetAll();
            var modelverifydata = _verifydatasqlrepository.GetByWhere("ModelID=19 and ModelMainID=@1", new object[] { modelid });
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
                var vdata = modelverifydata.Where(v => v.ModelItemID == d.ItemID);
                Paging.rows.Add(new PatentItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    IsRange = isrange == true ? "是" : "否",
                    GroupName = gname.Count() > 0 ? gname.First().Group_Name : "無分類",
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy/MM/dd"),
                    Sort = d.Sort.Value,
                    VerifyStr = vdata.Count() == 0 ? (d.IsVerift.Value ? "已通過" : "審核中") : vdata.First().VerifyStatus == 0 ? "審核中" : (vdata.First().VerifyStatus == 1 ? "已通過" : "未通過"),
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
                var oldmodel = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<PatentItem> mtseqdata = null;
                    mtseqdata = _itemsqlrepository.GetByWhere("Sort>@1 and ModelID=@2", new object[] { seq, modelid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _itemsqlrepository.GetCountUseWhere("ModelID=@1", new object[] { modelid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<PatentItem> ltseqdata = null;
                    ltseqdata = _itemsqlrepository.GetByWhere("Sort<=@1 and ModelID=@2", new object[] { qseq, modelid }).OrderBy(v => v.Sort).ToArray();
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
                        _itemsqlrepository.Update(tempmodel);
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _itemsqlrepository.Update(oldmodel.First());
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
                NLogManagement.SystemLogInfo("更新PatentItem排序失敗:" + " error:" + ex.Message);
                return "更新PatentItem排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region SetItemStatus
        public string SetItemStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new PatentItem();
                entity.Enabled = status ? true : false;
                entity.ItemID = int.Parse(id);
                var r = _itemsqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改PatentItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改PatentItem顯示狀態失敗:" + ex.Message);
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
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\MessageItem\\";
                var modelid = -1;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new PatentItem();
                    var olditem = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { idlist[idx] });
                    modelid = olditem.First().ModelID.Value;
                    entity.ItemID = int.Parse(idlist[idx]);
                    r = _itemsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除訊息項目失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        _verifydatasqlrepository.DelDataUseWhere("ModelID=19 and ModelMainID=@1 and ModelItemID=@2", new object[] { modelid, idlist[idx] });
                        _patentdetailsqlrepository.DelDataUseWhere("ItemID=@1", new object[] { idlist[idx] });
                        _itemsqlrepository.DelDataUseWhere("TypeName='MessageItem' and TypeID=@1", new object[] { idlist[idx] });
                        if (olditem.First().UploadFileName.IsNullorEmpty() == false)
                        {
                            if (System.IO.File.Exists(olditem.First().UploadFilePath))
                            {
                                System.IO.File.Delete(olditem.First().UploadFilePath);
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
                var alldata = _itemsqlrepository.GetByWhere("ModelId=@1", new object[] { modelid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    _itemsqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { idx, tempmodel.ItemID });
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

        #region GetModelByID
        public PatentEditModel GetModelByID(string modelid, string itemid)
        {
            var data = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { modelid });
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var model = new PatentEditModel()
                {
                    ItemID = fdata.ItemID,
                    ModelName = maindata.Count() == 0 ? "" : maindata.First().Name,
                    UploadFileDesc = fdata.UploadFileDesc,
                    UploadFileName = fdata.UploadFileName,
                    UploadFilePath = fdata.UploadFilePath,                  
                    HtmlContent = fdata.HtmlContent,
                    LinkUrl = fdata.LinkUrl,
                    ModelID = fdata.ModelID.Value,
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
                     Year=fdata.Year,
                     Inventor=fdata.Inventor,
                      Field=fdata.Field
                };
                var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                     19, fdata.ModelID.Value,fdata.ItemID
                });
                if (hasvdata.Count() > 0)
                {
                    model.VerifyStatus = hasvdata.First().VerifyStatus == 0 ? "審核中" : (hasvdata.First().VerifyStatus == 1 ? "已通過" : "未通過");
                    model.VerifyDateTime = hasvdata.First().VerifyDateTime == null ? "" : hasvdata.First().VerifyDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                    model.VerifyUser = hasvdata.First().VerifyName;
                }
                var detaildata = _patentdetailsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
                var NationArr = new List<string>();
                var PatentnoArr = new List<string>();
                var PatentDateArr = new List<string>();
                var EarlyPublicDateArr = new List<string>();
                var EarlyPublicNoArr = new List<string>();
                var DeadlineArr = new List<string>();
                foreach (var d in detaildata) {
                    NationArr.Add(d.Nation == null ? "" : d.Nation);
                    PatentnoArr.Add(d.Patentno == null ? "" : d.Patentno);
                    PatentDateArr.Add(d.PatentDate == null ? "" : d.PatentDate);
                    EarlyPublicDateArr.Add(d.EarlyPublicDate == null ? "" : d.EarlyPublicDate);
                    EarlyPublicNoArr.Add(d.EarlyPublicNo == null ? "" : d.EarlyPublicNo);
                    DeadlineArr.Add(d.Deadline == null ? "" : d.Deadline);
                }
                model.Nation = NationArr.ToArray();
                model.Patentno = PatentnoArr.ToArray();
                model.PatentDate = PatentDateArr.ToArray();
                model.EarlyPublicDate = EarlyPublicDateArr.ToArray();
                model.EarlyPublicNo = EarlyPublicNoArr.ToArray();
                model.Deadline = DeadlineArr.ToArray();
                return model;
            }
            else
            {
                PatentEditModel model = new PatentEditModel();
                model.ModelID = int.Parse(modelid);
                model.ModelName = maindata.Count() == 0 ? "" : maindata.First().Name;
                return model;
            }

        }
        #endregion

        #region CreateItem
        public string CreateItem(PatentEditModel model, string languageID, string account)
        {
            //var accountdata=
            var olddata = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { model.ModelID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new PatentItem()
            {
                HtmlContent = model.HtmlContent,
                ModelID = model.ModelID,
                LinkUrl = model.LinkUrl,
                UploadFileDesc = model.UploadFileDesc,
                UploadFileName = model.UploadFileName,
                Sort = 1,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(languageID),
                Title = model.Title,
                CreateDatetime = DateTime.Now,
                CreateUser = account,
                Enabled = true,
                Link_Mode = model.Link_Mode,
                CreateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = false,
                LinkUrlDesc = model.LinkUrlDesc,
                  Inventor=model.Inventor==null?"": model.Inventor,
                   Field = model.Field == null ? "" : model.Field,
                    Year = model.Year == null ? "" : model.Year
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
            var r = _itemsqlrepository.Create(savemodel);
            if (r > 0)
            {  
                if (model.Nation != null) {
                    for (var idx = 0; idx < model.Nation.Length; idx++) {
                        _patentdetailsqlrepository.Create(new PatentDetail()
                        {
                            Deadline = model.Deadline[idx],
                            EarlyPublicNo = model.EarlyPublicNo[idx],
                            EarlyPublicDate = model.EarlyPublicDate[idx],
                            GroupID = model.Group_ID,
                            ItemID = savemodel.ItemID,
                            LangID = int.Parse(languageID),
                            MainID = savemodel.ModelID,
                            Nation = model.Nation[idx],
                            PatentDate = model.PatentDate[idx],
                            Patentno = model.Patentno[idx],
                        });
                    }
                }

                _verifydatasqlrepository.DelDataUseWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] { 19, savemodel.ModelID.Value, savemodel.ItemID });
                _verifydatasqlrepository.Create(new VerifyData()
                {
                    ModelID =19,
                    ModelItemID = savemodel.ItemID,
                    ModelName = savemodel.Title,
                    ModelMainID = savemodel.ModelID.Value,
                    VerifyStatus = 0,
                    ModelStatus = 1,
                    UpdateDateTime = DateTime.Now,
                    UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                    UpdateAccount = account,
                    LangID = int.Parse(languageID)
                });

                foreach (var odata in olddata)
                {
                    _itemsqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { odata.Sort + 1, odata.ItemID });
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
        public string UpdateItem(PatentEditModel model, string languageID, string account)
        {
            var olddata = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PatentItem\\";
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
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new PatentItem()
            {
                ItemID = model.ItemID,
                HtmlContent = model.HtmlContent == null ? "" : model.HtmlContent,
                ModelID = model.ModelID,
                LinkUrl = model.LinkUrl == null ? "" : model.LinkUrl == null ? "" : model.LinkUrl,
                LinkUrlDesc = model.LinkUrlDesc == null ? "" : model.LinkUrlDesc == null ? "" : model.LinkUrlDesc,
                UploadFileDesc = model.UploadFileDesc == null ? "" : model.UploadFileDesc,
                UploadFileName = model.UploadFileName,
                UploadFilePath = model.UploadFilePath == null ? "" : model.UploadFilePath,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(languageID),
                Title = model.Title == null ? "" : model.Title,
                UpdateDatetime = DateTime.Now,
                UpdateUser = account,
                Enabled = true,
                Link_Mode = model.Link_Mode,
                UpdateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = false,
                Inventor = model.Inventor == null ? "" : model.Inventor,
                Field = model.Field == null ? "" : model.Field,
                Year = model.Year == null ? "" : model.Year
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

            var r = _itemsqlrepository.Update(savemodel);
            if (r > 0)
            {
                _patentdetailsqlrepository.DelDataUseWhere("MainID=@1 and ItemID=@2", new object[] { model.ModelID, model.ItemID });
                if (model.Nation != null)
                {
                    for (var idx = 0; idx < model.Nation.Length; idx++)
                    {
                        _patentdetailsqlrepository.Create(new PatentDetail()
                        {
                            Deadline = model.Deadline[idx],
                            EarlyPublicNo = model.EarlyPublicNo[idx],
                            EarlyPublicDate = model.EarlyPublicDate[idx],
                            GroupID = model.Group_ID,
                            ItemID = model.ItemID,
                            LangID = int.Parse(languageID),
                            MainID = model.ModelID,
                            Nation = model.Nation[idx],
                            PatentDate = model.PatentDate[idx],
                            Patentno = model.Patentno[idx],
                        });
                    }
                }
                var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                     19, savemodel.ModelID.Value,savemodel.ItemID
                });
                if (hasvdata.Count() == 0)
                {
                    _verifydatasqlrepository.Create(new VerifyData()
                    {
                        ModelID = 19,
                        ModelItemID = savemodel.ItemID,
                        ModelName = savemodel.Title,
                        ModelMainID = savemodel.ModelID.Value,
                        VerifyStatus = 0,
                        ModelStatus = 2,
                        UpdateDateTime = DateTime.Now,
                        UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                        UpdateAccount = account,
                        LangID = int.Parse(languageID)
                    });
                }
                else
                {
                    _verifydatasqlrepository.Update("VerifyStatus=0,ModelStatus=2,VerifyDateTime=Null,VerifyUser='',VerifyName='',ModelName=@1,UpdateDateTime=@2,UpdateUser=@3,UpdateAccount=@4",
                        "ModelID=@5 and ModelMainID=@6 and ModelItemID=@7", new object[] {
                           savemodel.Title,DateTime.Now, (admin.Count() == 0 ? "" : admin.First().User_Name),account,19 , savemodel.ModelID.Value, savemodel.ItemID
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

        #region GetUnitModel
        public PatentUnitSettingModel GetUnitModel(string modelid)
        {
            var data = _unitsqlitemrepository.GetByWhere("MainID=@1", new object[] { modelid });
            var model = new PatentUnitSettingModel();
            model.MainID = int.Parse(modelid);
            if (data.Count() > 0)
            {
                model = new PatentUnitSettingModel()
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
                    IntroductionHtml=data.First().IntroductionHtml
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
                var columnlist = _columnsqlrepository.GetByWhere("Type='Patent'", null).OrderBy(v => v.Sort);
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
            model.ColumnNameMapping.Add("年度", model.Column1.IsNullorEmpty() ? "年度" : model.Column1);
            model.ColumnNameMapping.Add("標題", model.Column2.IsNullorEmpty() ? "標題" : model.Column2);
            model.ColumnNameMapping.Add("類別", model.Column3.IsNullorEmpty() ? "類別" : model.Column3);
            model.ColumnNameMapping.Add("發明人", model.Column4.IsNullorEmpty() ? "發明人" : model.Column4);
            model.ColumnNameMapping.Add("領域", model.Column5.IsNullorEmpty() ? "領域" : model.Column5);
            model.ColumnNameMapping.Add("簡介", model.Column6.IsNullorEmpty() ? "簡介" : model.Column6);
            model.ColumnNameMapping.Add("國別", model.Column7.IsNullorEmpty() ? "國別" : model.Column7);
            model.ColumnNameMapping.Add("專利證書號", model.Column8.IsNullorEmpty() ? "專利證書號" : model.Column8);
            model.ColumnNameMapping.Add("證書日期", model.Column9.IsNullorEmpty() ? "證書日期" : model.Column9);
            model.ColumnNameMapping.Add("早期公開日", model.Column10.IsNullorEmpty() ? "早期公開日" : model.Column10);
            model.ColumnNameMapping.Add("早期公開號", model.Column11.IsNullorEmpty() ? "早期公開號" : model.Column11);
            model.ColumnNameMapping.Add("專利權限期", model.Column12.IsNullorEmpty() ? "專利權限期" : model.Column12);
            model.ColumnNameMapping.Add("專利明細列表", model.Column13.IsNullorEmpty() ? "專利明細列表" : model.Column13);
            
            return model;
        }
        #endregion

        #region SetUnitModel
        public string SetUnitModel(PatentUnitSettingModel model, string account)
        {
            var newmodel = new PatentUnitSetting();
            newmodel.UpdateDatetime = DateTime.Now;
            newmodel.UpdateUser = account;
            var columnsetting ="";
            if (model.ColumnName!=null) {
                columnsetting = string.Join(",", model.ColumnName) + "@" + string.Join(",", model.ColumnUse);
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
                newmodel.Column8 = model.Column8;
                newmodel.Column9 = model.Column9;
                newmodel.Column10 = model.Column10;
                newmodel.Column11 = model.Column11;
                newmodel.Column12 = model.Column12;
                newmodel.Column13 = model.Column13;
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
                newmodel.IntroductionHtml = model.IntroductionHtml==null?"": model.IntroductionHtml;
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

        #region GetModelItem
        public PatentItem GetModelItem(string itemid)
        {
            var data = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            if (data.Count() > 0)
            {
                return data.First();
            }
            else
            {
                return new PatentItem();
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

        #region PagingItemForWebSite
        public Paging<PatentItemResult> PagingItemForWebSite(string modelid, PatentSearchModel model, string nogroupstr)
        {
            var Paging = new Paging<PatentItemResult>();
            var data = new List<PatentItem>();

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
                    data = _itemsqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
                }
                else
                {
                    data = _itemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                }

                Paging.total = _itemsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            }
            else
            {
                if (model.Limit == -1)
                {
                    data = _itemsqlrepository.GetAll("*", model.Sort).ToList();
                }
                else
                {
                    data = _itemsqlrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
                }
                Paging.total = _itemsqlrepository.GetAllDataCount();

            }
            var sidx = model.Offset;
            foreach (var d in data)
            {

                var gname = ALLGroup.Where(v => v.ID == d.GroupID);

                sidx += 1;
                Paging.rows.Add(new PatentItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    GroupName = gname.Count() > 0 ? gname.First().Group_Name : nogroupstr,
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy.MM.dd"),
                    Sort = sidx,
                    UploadFileName = d.UploadFileName == null ? "" : d.UploadFileName,
                    Year = d.Year
                });

            }
            return Paging;
        }
        #endregion
    }
}
