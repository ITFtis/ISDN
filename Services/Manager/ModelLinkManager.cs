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
    public class ModelLinkManager : IModelLinkManager
    {
        readonly SQLRepository<PageActiveItem> _activeitemsqlrepository;
        readonly SQLRepository<LinkItem> _sqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        readonly SQLRepository<VerifyData> _verifydatasqlrepository;
        public ModelLinkManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.LinkItem;
            _Usersqlrepository = sqlinstance.Users;
            _verifydatasqlrepository = sqlinstance.VerifyData;
            _activeitemsqlrepository = sqlinstance.PageActiveItem;
        }

        #region CreateItem
        public string CreateItem(LinkEditModel model, string LangId, string account)
        {
            var olddata = _sqlrepository.GetByWhere("Lang_ID=@1", new object[] { LangId });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new LinkItem()
            {
                ImageFileDesc = model.ImageFileDesc == null ? "" : model.ImageFileDesc,
                ImageFileOrgName = model.ImageFileOrgName,
                LinkUrl = model.LinkUrl,
                ImageFileName = model.ImageFileName,
                Sort = 1,
                GroupID = model.Group_ID,
                Lang_ID = int.Parse(LangId),
                Title = model.Title,
                CreateDatetime = DateTime.Now,
                CreateUser = account,
                Enabled = true,
                Link_Mode = model.Link_Mode,
                Introduction = model.Introduction == null ? "" : model.Introduction,
                CreateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift = true,
                LinkUrlDesc = model.LinkUrlDesc == null ? "" : model.LinkUrlDesc,
                Type=model.Type
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
            var r = _sqlrepository.Create(savemodel);
            if (r > 0)
            {
                foreach (var odata in olddata)
                {
                    _sqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { odata.Sort + 1, odata.ItemID });
                }
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }
        }

        #endregion

        #region DeleteItem
        public string DeleteItem(string[] idlist, string delaccount, string account, string username)
        {
            try
            {
                var r = 0;
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\LinkItem\\";
                var langid = -1;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new LinkItem();
                    var olditem = _sqlrepository.GetByWhere("ItemID=@1", new object[] { idlist[idx] });
                    langid = olditem.First().Lang_ID.Value;
                    entity.ItemID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除連結項目失敗:ID=" + idlist[idx]);
                    }
                    else
                    {
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
                var alldata = _sqlrepository.GetByWhere("Lang_ID=@1", new object[] { langid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    _sqlrepository.Update("Sort=@1", "ItemID=@2", new object[] { idx, tempmodel.ItemID });
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
        public LinkEditModel GetModelByID(string modelid, string itemid)
        {
            var data = _sqlrepository.GetByWhere("ItemID=@1", new object[] { itemid });
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var model = new LinkEditModel()
                {
                    ItemID = fdata.ItemID,
                    ImageFileName = fdata.ImageFileName,
                    ImageFileOrgName = fdata.ImageFileOrgName,
                    ImageFileDesc = fdata.ImageFileDesc,
                    LinkUrl = fdata.LinkUrl,
                    ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/LinkItem/" + fdata.ImageFileName),
                    EdDate = fdata.EdDate,
                    EdDateStr = fdata.EdDate == null ? "" : fdata.EdDate.Value.ToString("yyyy/MM/dd"),
                    Group_ID = fdata.GroupID == null ? -1 : fdata.GroupID.Value,
                    Link_Mode = fdata.Link_Mode,
                    PublicshStr = fdata.PublicshDate == null ? "" : fdata.PublicshDate.Value.ToString("yyyy/MM/dd"),
                    StDate = fdata.StDate,
                    StDateStr = fdata.StDate == null ? "" : fdata.StDate.Value.ToString("yyyy/MM/dd"),
                    Title = fdata.Title,
                    Introduction = fdata.Introduction,
                    CreateDatetime = fdata.CreateDatetime == null ? "" : fdata.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateUser = fdata.CreateName,
                    UpdateDatetime = fdata.UpdateDatetime == null ? "" : fdata.UpdateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateUser = fdata.UpdateName,
                    LinkUrlDesc = fdata.LinkUrlDesc
                };
                return model;
            }
            else
            {
                LinkEditModel model = new LinkEditModel();
                return model;
            }

        }
        #endregion

        #region PagingItem
        public Paging<LinkItemResult> PagingItem(string isfront, SearchModelBase model)
        {
            var Paging = new Paging<LinkItemResult>();
            var data = new List<LinkItem>();

            var whereobj = new List<object>();
            var wherestr = new List<string>();
            var idx = 1;
            wherestr.Add("Lang_ID=@1");
            whereobj.Add(model.LangId);
            var nowdate = DateTime.Now;
            if (model.Key.IsNullorEmpty() == false)
            {
                idx += 1;
                wherestr.Add("Type=@" + idx);
                whereobj.Add(model.Key);
            }
            if (isfront == "Y")
            {
                idx += 1;
                wherestr.Add("Enabled=@" + idx);
                whereobj.Add(true);

                idx += 1;
                wherestr.Add("(StDate <=@" + idx + " or StDate is null or StDate='')");
                whereobj.Add(nowdate);

                idx += 1;
                wherestr.Add("(EdDate >=@" + idx + " or EdDate is null or EdDate='')");
                whereobj.Add(nowdate);

                idx += 1;
                wherestr.Add("(PublicshDate <=@" + idx + " or PublicshDate is null or PublicshDate='')");
                whereobj.Add(nowdate);
            }
            if (model.Limit != -1)
            {
                var str = string.Join(" and ", wherestr);
                data = _sqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
                Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());

            }
            else
            {
                var str = string.Join(" and ", wherestr);
                Paging.total = _sqlrepository.GetAllDataCount();
                data = _sqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
            }
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
                Paging.rows.Add(new LinkItemResult()
                {
                    ItemID = d.ItemID,
                    Title = d.Title,
                    Enabled = d.Enabled,
                    IsRange = isrange == true ? "是" : "否",
                    PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy/MM/dd"),
                    Sort = d.Sort.Value,
                    LinkUrl = d.LinkUrl,
                    Link_Mode = d.Link_Mode,
                    ImageFileName = d.ImageFileName == null ? "" : d.ImageFileName,
                });

            }
            return Paging;
        } 
        #endregion

        #region SetItemStatus
        public string SetItemStatus(string id, bool status, string account, string username)
        {
            try
            {
                var entity = new LinkItem();
                entity.Enabled = status ? true : false;
                entity.ItemID = int.Parse(id);
                var r = _sqlrepository.Update(entity);
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("修改LinkItem顯示狀態 id=" + id + " 為:" + status);
                    return "更新成功";
                }
                else
                {
                    return "更新失敗";
                }

            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("修改LinkItem顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region UpdateItem
        public string UpdateItem(LinkEditModel model, string LangId, string account)
        {
            var olddata = _sqlrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\LinkItem\\";
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
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var savemodel = new LinkItem()
            {
                ItemID = model.ItemID,
                ImageFileDesc = model.ImageFileDesc == null ? "" : model.ImageFileDesc,
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
                IsVerift = false
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

            var r = _sqlrepository.Update(savemodel);
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

        #region UpdateItemSeq
        public string UpdateItemSeq(int langid, int id, int seq, string account, string username,string type)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _sqlrepository.GetByWhere("ItemID=@1", new object[] { id});
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<LinkItem> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and Lang_ID=@2 and Type=@3", new object[] { seq, langid, type }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1 and Type=@2", new object[] { langid , type });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<LinkItem> ltseqdata = null;
                    ltseqdata = _sqlrepository.GetByWhere("Sort<=@1 and Lang_ID=@2  and Type=@3", new object[] { qseq, langid, type }).OrderBy(v => v.Sort).ToArray();
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
                NLogManagement.SystemLogInfo("更新LinkItem排序失敗:" + " error:" + ex.Message);
                return "更新LinkItem排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        //#region PagingItem
        //public Paging<PageActiveResult> PagingItem(string isfront, SearchModelBase model)
        //{
        //    var Paging = new Paging<PageActiveResult>();
        //    var data = new List<PageActiveItem>();

        //    var whereobj = new List<object>();
        //    var wherestr = new List<string>();
        //    var idx = 1;
        //    wherestr.Add("Lang_ID=@1");
        //    whereobj.Add(model.LangId);
        //    var nowdate = DateTime.Now;
        //    if (isfront == "Y")
        //    {
        //        idx += 1;
        //        wherestr.Add("Enabled=@" + idx);
        //        whereobj.Add(true);

        //        idx += 1;
        //        wherestr.Add("(StDate <=@" + idx + " or StDate is null or StDate='')");
        //        whereobj.Add(nowdate);

        //        idx += 1;
        //        wherestr.Add("(EdDate >=@" + idx + " or EdDate is null or EdDate='')");
        //        whereobj.Add(nowdate);

        //        idx += 1;
        //        wherestr.Add("(PublicshDate <=@" + idx + " or PublicshDate is null or PublicshDate='')");
        //        whereobj.Add(nowdate);
        //    }
        //    if (model.Limit != -1)
        //    {
        //        var str = string.Join(" and ", wherestr);
        //        data = _activeitemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort).ToList();
        //        Paging.total = _sqlrepository.GetCountUseWhere(str, whereobj.ToArray());

        //    }
        //    else
        //    {
        //        var str = string.Join(" and ", wherestr);
        //        Paging.total = _sqlrepository.GetAllDataCount();
        //        data = _activeitemsqlrepository.GetByWhere(str + " order by " + model.Sort, whereobj.ToArray()).ToList();
        //    }
        //    foreach (var d in data)
        //    {
        //        var isrange = false;
        //        if (d.StDate == null && d.EdDate == null) { isrange = true; }
        //        else if (d.StDate != null && d.EdDate == null)
        //        {
        //            if (DateTime.Now >= d.StDate.Value)
        //            {
        //                isrange = true;
        //            }
        //        }
        //        else if (d.StDate == null && d.EdDate != null)
        //        {
        //            if (DateTime.Now <= d.EdDate.Value.AddDays(1))
        //            {
        //                isrange = true;
        //            }
        //        }
        //        else if (d.StDate != null && d.EdDate != null)
        //        {
        //            if (DateTime.Now >= d.StDate.Value && DateTime.Now <= d.EdDate.Value.AddDays(1))
        //            {
        //                isrange = true;
        //            }
        //        }
        //        Paging.rows.Add(new PageActiveResult()
        //        {
        //            ItemID = d.ItemID,
        //            Title = d.Title,
        //            Enabled = d.Enabled,
        //            IsRange = isrange == true ? "是" : "否",
        //            PublicshDate = d.PublicshDate == null ? "" : d.PublicshDate.Value.ToString("yyyy/MM/dd"),
        //            Sort = d.Sort.Value,
        //            LinkUrl = d.LinkUrl,
        //            Link_Mode = d.Link_Mode,
        //            ImageFileName = d.ImageFileName == null ? "" : d.ImageFileName,
        //        });

        //    }
        //    return Paging;
        //} 
        //#endregion
    }
}
