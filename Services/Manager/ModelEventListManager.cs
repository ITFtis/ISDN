using Services.Interface;
using SQLModel;
using SQLModel.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Utilities;
using ViewModels;
using ViewModels.DBModels;

namespace Services.Manager
{
    public class ModelEventListManager : IModelEventListManager
    {
        readonly SQLRepository<ModelEventListMain> _sqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<VerifyData> _verifydatasqlrepository;
        readonly SQLRepository<EventListItem> _sqlitemrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        public ModelEventListManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelEventListMain;
            _menusqlrepository = sqlinstance.Menu;
            _verifydatasqlrepository = sqlinstance.VerifyData;
            _sqlitemrepository = sqlinstance.EventListItem;
            _seosqlrepository = sqlinstance.SEO;
            _Usersqlrepository = sqlinstance.Users;
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
            var Model = new ModelEventListMain();
            Model.Lang_ID = int.Parse(langid);
            Model.ModelID = 17;
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

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var m = _menusqlrepository.GetByWhere("ModelID=17 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelEventListMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除大事記要失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        _verifydatasqlrepository.DelDataUseWhere("ModelID=17 and ModelMainID=@1", new object[] { idlist[idx] });
                        _seosqlrepository.DelDataUseWhere("TypeName='ActiveItem' and TypeID=@1", new object[] { idlist[idx] });
                        var olditem = _sqlitemrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                        var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EventListItem\\";
                        foreach (var items in olditem)
                        {
                            _sqlitemrepository.Delete(items);
                            _seosqlrepository.DelDataUseWhere("TypeName='EventListItem' and TypeID=@1", new object[] { items.ItemID });
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
                    NLogManagement.SystemLogInfo("刪除大事記要失敗:" + delaccount);
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
                NLogManagement.SystemLogInfo("刪除大事記要失敗:" + ex.Message);
                return "刪除失敗";
            }
        } 
        #endregion

        #region GetAll
        public IEnumerable<ModelEventListMain> GetAll()
        {
            return _sqlrepository.GetAll();
        } 
        #endregion

        #region Paging
        public Paging<ModelEventListMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelEventListMain>();
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

        #region PagingItem
        public Paging<EventListItemResult> PagingItem(string modelid, EventListSearchModel model)
        {
            var Paging = new Paging<EventListItemResult>();
            var data = new List<EventListItem>();

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
                data = _sqlitemrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _sqlitemrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                Paging.total = _sqlitemrepository.GetAllDataCount();
                data = _sqlitemrepository.GetAllDataRange(model.Offset, model.Limit, model.Sort).ToList();
            }

            var modelverifydata = _verifydatasqlrepository.GetByWhere("ModelID=17 and ModelMainID=@1", new object[] { modelid });
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
                var vdata = modelverifydata.Where(v => v.ModelItemID == d.ItemID);
                Paging.rows.Add(new EventListItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    ClickCount = d.ClickCnt == null ? "0" : d.ClickCnt.Value.ToString(),
                    Enabled = d.Enabled,
                    IsRange = isrange == true ? "是" : "否",
                    Year = d.Year,
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy/MM/dd"),
                    Sort = d.Sort.Value,
                    VerifyStr = vdata.Count() == 0 ? (d.IsVerift.Value ? "已通過" : "審核中") : vdata.First().VerifyStatus == 0 ? "審核中" : (vdata.First().VerifyStatus == 1 ? "已通過" : "未通過"),
                });

            }
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
                    IList<ModelEventListMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and Lang_ID=@2", new object[] { seq, langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1", new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelEventListMain> ltseqdata = null;
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
                NLogManagement.SystemLogInfo("更新ModelEventListMain排序失敗:" + " error:" + ex.Message);
                return "更新ModelEventListMain排序失敗:" + " error:" + ex.Message;
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
        public IEnumerable<ModelEventListMain> Where(ModelEventListMain model)
        {
            return _sqlrepository.GetByWhere(model);
        }
        #endregion

        #region UpdateItemSeq
        public string UpdateItemSeq(int modelid, int id, int seq, string account, string username)
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
                    IList<EventListItem> mtseqdata = null;
                    mtseqdata = _sqlitemrepository.GetByWhere("Sort>@1 and ModelID=@2", new object[] { seq, modelid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlitemrepository.GetCountUseWhere("ModelID=@1", new object[] { modelid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<EventListItem> ltseqdata = null;
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
                NLogManagement.SystemLogInfo("更新EventListItem排序失敗:" + " error:" + ex.Message);
                return "更新EventListItem排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region DeleteItem
        public string DeleteItem(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EventListItem\\";
                var modelid = -1;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new EventListItem();
                    var olditem = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { idlist[idx] });
                    modelid = olditem.First().ModelID.Value;
                    entity.ItemID = int.Parse(idlist[idx]);
                    r = _sqlitemrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除訊息項目失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
                        _verifydatasqlrepository.DelDataUseWhere("ModelID=17 and ModelMainID=@1 and ModelItemID=@2", new object[] { modelid, idlist[idx] });

                        _seosqlrepository.DelDataUseWhere("TypeName='EventListItem' and TypeID=@1", new object[] { idlist[idx] });
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
                    _sqlitemrepository.Update("Sort=@1", "ItemID=@2", new object[] { idx, tempmodel.ItemID });
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
                var entity = new EventListItem();
                entity.Enabled = status ? true : false;
                entity.ItemID = int.Parse(id);
                var r = _sqlitemrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改EventListItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改EventListItem顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region CreateItem
        public string CreateItem(EventListEditModel model, string LangId, string account)
        {
            var iswriteseo = false;
            //var accountdata=
            var olddata = _sqlitemrepository.GetByWhere("ModelID=@1", new object[] { model.ModelID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new EventListItem()
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
                Link_Mode = model.Link_Mode,
                RelateImageFileName = model.RelateImageName,
                RelateImageFileOrgName = model.RelateImageFileOrgName,
                Introduction = model.Introduction == null ? "" : model.Introduction,
                CreateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = false,
                LinkUrlDesc = model.LinkUrlDesc,
                 Year=model.Year
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
            var r = _sqlitemrepository.Create(savemodel);
            if (r > 0)
            {
                _verifydatasqlrepository.DelDataUseWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] { 17, savemodel.ModelID.Value, savemodel.ItemID });
                _verifydatasqlrepository.Create(new VerifyData()
                {
                    ModelID = 17,
                    ModelItemID = savemodel.ItemID,
                    ModelName = savemodel.Title,
                    ModelMainID = savemodel.ModelID.Value,
                    VerifyStatus = 0,
                    ModelStatus = 1,
                    UpdateDateTime = DateTime.Now,
                    UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                    UpdateAccount = account,
                    LangID = int.Parse(LangId)
                });

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
                        TypeName = "MessageItem",
                        TypeID = savemodel.ItemID,
                        Lang_ID = int.Parse(LangId)
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

        #region UpdateItem
        public string UpdateItem(EventListEditModel model, string LangId, string account)
        {
            var iswriteseo = false;
            if (model.WebsiteTitle.IsNullorEmpty() == false || model.Description.IsNullorEmpty() == false
                   || (model.Keywords != null && model.Keywords.Any(v => v.IsNullorEmpty() == false)))
            {
                iswriteseo = true;
            }
            var olddata = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\EventListItem\\";

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

            _seosqlrepository.DelDataUseWhere("TypeName='MessageItem' and TypeID=@1", new object[] { model.ItemID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new EventListItem()
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
                UploadFileDesc = model.UploadFileDesc == null ? "" : model.UploadFileDesc,
                UploadFileName = model.UploadFileName,
                UploadFilePath = model.UploadFilePath == null ? "" : model.UploadFilePath,
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
                IsVerift = false,
                Year=model.Year
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

            var r = _sqlitemrepository.Update(savemodel);
            if (r > 0)
            {
                var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                     17, savemodel.ModelID.Value,savemodel.ItemID
                });
                if (hasvdata.Count() == 0)
                {
                    _verifydatasqlrepository.Create(new VerifyData()
                    {
                        ModelID =17,
                        ModelItemID = savemodel.ItemID,
                        ModelName = savemodel.Title,
                        ModelMainID = savemodel.ModelID.Value,
                        VerifyStatus = 0,
                        ModelStatus = 2,
                        UpdateDateTime = DateTime.Now,
                        UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                        UpdateAccount = account,
                        LangID = int.Parse(LangId)
                    });
                }
                else
                {
                    _verifydatasqlrepository.Update("VerifyStatus=0,ModelStatus=2,VerifyDateTime=Null,VerifyUser='',VerifyName='',ModelName=@1,UpdateDateTime=@2,UpdateUser=@3,UpdateAccount=@4",
                        "ModelID=@5 and ModelMainID=@6 and ModelItemID=@7", new object[] {
                           savemodel.Title,DateTime.Now, (admin.Count() == 0 ? "" : admin.First().User_Name),account,17 , savemodel.ModelID.Value, savemodel.ItemID
                    });
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
                        TypeName = "MessageItem",
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

        #region GetModelByID
        public EventListEditModel GetModelByID(string modelid, string itemid)
        {
            var data = _sqlitemrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { modelid });
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var seodata = _seosqlrepository.GetByWhere("TypeName='EventListEditModel' and TypeID=@1", new object[] { itemid });
                var model = new EventListEditModel()
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
                    ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/EventListItem/" + fdata.ImageFileName),
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
                    RelateImagelUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/EventListItem/" + fdata.RelateImageFileName),
                    Introduction = fdata.Introduction,
                    CreateDatetime = fdata.CreateDatetime == null ? "" : fdata.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateUser = fdata.CreateName,
                    UpdateDatetime = fdata.UpdateDatetime == null ? "" : fdata.UpdateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateUser = fdata.UpdateName,
                    LinkUrlDesc = fdata.LinkUrlDesc,
                    Year=fdata.Year
                };
                var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                    17, fdata.ModelID.Value,fdata.ItemID
                });
                if (hasvdata.Count() > 0)
                {
                    model.VerifyStatus = hasvdata.First().VerifyStatus == 0 ? "審核中" : (hasvdata.First().VerifyStatus == 1 ? "已通過" : "未通過");
                    model.VerifyDateTime = hasvdata.First().VerifyDateTime == null ? "" : hasvdata.First().VerifyDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                    model.VerifyUser = hasvdata.First().VerifyName;
                }
                return model;
            }
            else
            {
                EventListEditModel model = new EventListEditModel();
                model.Keywords = new string[10];
                model.ModelID = int.Parse(modelid);
                model.ImageFileLocation = "1";
                model.ModelName = maindata.Count() == 0 ? "" : maindata.First().Name;
                return model;
            }

        }

        #endregion

        #region GetItemList
        public IList<EventListItem> GetModelIDList(string modelid)
        {
            //ModelID=@1 and GroupID In(22,23,0)
            return _sqlitemrepository.GetByWhere("ModelID=@1  and Enabled=@2 and (StDate <=@3 or StDate is null or StDate='') "+
                "and (EdDate >=@3 or EdDate is null or EdDate='') and IsVerift=1 ", new object[] { modelid,1,DateTime.Now }).ToList();
        } 
        #endregion
    }
}
