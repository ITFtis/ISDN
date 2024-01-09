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
using ViewModels.DBModels;

namespace Services.Manager
{
    public class ModelPageEditManager : IModelPageEditManager
    {
        readonly SQLRepository<ModelPageEditMain> _sqlrepository;
        readonly SQLRepository<PageIndexItem> _itemsqlrepository;
        readonly SQLRepository<SEO> _seosqlrepository;
        readonly SQLRepository<PageUnitSetting> _unitsqlrepository;
        readonly SQLRepository<Menu> _menusqlrepository;
        readonly SQLRepository<PageLayout> _pagesqlrepository;
       // readonly SQLRepository<VerifyData> _verifydatasqlrepository;
        readonly SQLRepository<Users> _Usersqlrepository;
        readonly SQLRepository<SiteConfig> _siteconfigqlrepository;
        public ModelPageEditManager(SQLRepositoryInstances sqlinstance)
        {
            _sqlrepository = sqlinstance.ModelPageEditMain;
            _seosqlrepository = sqlinstance.SEO;
            _itemsqlrepository = sqlinstance.PageIndexItem;
            _unitsqlrepository = sqlinstance.PageUnitSetting;
            _menusqlrepository = sqlinstance.Menu;
            _pagesqlrepository = sqlinstance.PageLayout;
           // _verifydatasqlrepository = sqlinstance.VerifyData;
            _Usersqlrepository = sqlinstance.Users;
            _siteconfigqlrepository = sqlinstance.SiteConfig;
        }

        #region GetAll
        public IEnumerable<ModelPageEditMain> GetAll()
        {
            return _sqlrepository.GetAll();
        }

        #endregion

        #region Paging
        public Paging<ModelPageEditMain> Paging(SearchModelBase model)
        {
            var Paging = new Paging<ModelPageEditMain>();
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

        #region Paging
        public Paging<PageIndexItem> PagingItem(SearchModelBase model)
        {
            var Paging = new Paging<PageIndexItem>();
            var onepagecnt = model.Limit;
            var whereobj = new List<string>();
            var wherestr = new List<string>();
            wherestr.Add("ModelID=@1");
            whereobj.Add(model.Key);
            if (model.Search != "")
            {
                if (string.IsNullOrEmpty(model.Search) == false)
                {
                    wherestr.Add("ItemName like @2");
                    whereobj.Add("%" + model.Search + "%");
                }
            }
            var str = string.Join(" and ", wherestr);
            Paging.rows = _itemsqlrepository.GetDataUseWhereRange(str, whereobj.ToArray(), model.Offset, model.Limit, model.Sort);
            Paging.total = _itemsqlrepository.GetCountUseWhere(str, whereobj.ToArray());
            return Paging;
        }
        #endregion

        #region Where
        public IEnumerable<ModelPageEditMain> Where(ModelPageEditMain model)
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
            var PageIndexMain = new ModelPageEditMain();
            PageIndexMain.Lang_ID = int.Parse(langid);
            PageIndexMain.Name = name;
            PageIndexMain.ModelID = 1;
            PageIndexMain.CreateDate = DateTime.Now;
            PageIndexMain.UpdateDate = DateTime.Now;
            PageIndexMain.CreateUser = account;
            PageIndexMain.UpdateUser = account;
            PageIndexMain.Sort = 1;
            PageIndexMain.Status = true;
            var r = _sqlrepository.Create(PageIndexMain);
            newid = PageIndexMain.ID;
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
            //r = _verifydatasqlrepository.Update("ModelName=@1", "ModelID=@2 and ModelMainID=@3", new object[] { name, 1, id });
            if (r > 0) {

                _itemsqlrepository.Update("ItemName=@1", "ModelID=@2", new object[] { name, id });
                return "修改成功"; } else { return "修改失敗"; }
        } 
        #endregion

        #region UpdateSeq
        public string UpdateSeq(int id, int seq, string langid ,string account, string username)
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
                    IList<ModelPageEditMain> mtseqdata = null;
                    mtseqdata = _sqlrepository.GetByWhere("Sort>@1 and Lang_ID=@2", new object[] { seq , langid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _sqlrepository.GetCountUseWhere("Lang_ID=@1",new object[] { langid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<ModelPageEditMain> ltseqdata = null;
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
                NLogManagement.SystemLogInfo("更新圖文編輯排序失敗:" + " error:" + ex.Message);
                return "更新圖文編輯排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region Delete
        public string Delete(string[] idlist, string delaccount, string langid, string account, string username)
        {
            try
            {
                var r = 0;
                //檢查是否在使用中
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var m = _menusqlrepository.GetByWhere("ModelID=1 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (m.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                    var p = _pagesqlrepository.GetByWhere("ModelID=1 and ModelItemID=@1", new object[] { idlist[idx] });
                    if (p.Count() > 0)
                    {
                        return "刪除失敗,刪除的項目使用中..";
                    }
                }
                var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PageEdit\\";
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new ModelPageEditMain();
                    entity.ID = int.Parse(idlist[idx]);
                    r = _sqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除圖文編輯單元失敗:ID=" + idlist[idx]);
                    }
                    //_verifydatasqlrepository.DelDataUseWhere("ModelID=@1and ModelMainID=@2", new object[] {1,idlist[idx]});
                    var allitems = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                    _seosqlrepository.DelDataUseWhere("TypeName='PageIndexItem' and TypeID=@1", new object[] { idlist[idx] });
                    _unitsqlrepository.DelDataUseWhere("MainID=@1", new object[] { idlist[idx] });
                    var olditem = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { idlist[idx] });
                    foreach (var items in olditem)
                    {
                        _itemsqlrepository.Delete(items);
                        _seosqlrepository.DelDataUseWhere("TypeName='PageIndexItem' and TypeID=@1", new object[] { items.ItemID });
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
                    }
                }
                var rstr = "";
                if (r >= 0)
                {
                    _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                    NLogManagement.SystemLogInfo("刪除圖文編輯單元失敗:" + delaccount);
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
                NLogManagement.SystemLogInfo("刪除圖文編輯單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region CreatePageEdit
        public string CreatePageEdit(PageEditItemModel model,string LangId,string account)
        {
            var iswriteseo = false;
            if (model.WebsiteTitle.IsNullorEmpty() == false || model.Description.IsNullorEmpty() == false
                   || model.Keywords.Any(v => v.IsNullorEmpty() == false))
            {
                iswriteseo = true;
            }
            var mexcoount = _itemsqlrepository.GetDataCaculate("Max(Sort)", "ModelID=@1", new object[] { model.ModelID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });        
            var PageIndexItem = new PageIndexItem()
            {
                 Lang_ID=int.Parse( LangId),
                 Enabled=true,
                 HtmlContent=model.HtmlContent,
                 ImageFileDesc=model.ImageFileDesc,
                 ImageFileLocation=model.ImageFileLocation,
                 UpdateDatetime=DateTime.Now,
                 ModelID=model.ModelID,
                 UpdateUser= account,
                 ImageFileOrgName = model.ImageFileOrgName,
                 LinkUrl = model.LinkUrl,
                 UploadFileDesc = model.UploadFileDesc,
                 UploadFileName = model.UploadFileName,
                 UploadFilePath = model.UploadFilePath,
                 ImageFileName = model.ImageFileName,
                 Sort= mexcoount+1,
                CreateDatetime = DateTime.Now,
                CreateUser = account,
                CreateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                LinkUrlDesc= model.LinkUrlDesc,
                IsVerift =true
            };
            if (model.ActiveID.IsNullorEmpty() == false)
            {
                PageIndexItem.ActiveID = int.Parse(model.ActiveID);
            }
            if (model.ActiveItemID.IsNullorEmpty() == false)
            {
                PageIndexItem.ActiveItemID = int.Parse(model.ActiveItemID);
            }
            var r = _itemsqlrepository.Create(PageIndexItem);
            if (r > 0)
            {
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                //_verifydatasqlrepository.Create(new VerifyData()
                //{
                //     ModelID=1,
                //       ModelItemID= PageIndexItem.ItemID.Value,
                //        ModelName= PageIndexItem.ItemName,
                //        ModelMainID = PageIndexItem.ModelID.Value,
                //         VerifyStatus =0,
                //         ModelStatus=1,
                //    UpdateDateTime = DateTime.Now,
                //    UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                //    UpdateAccount = account,
                //    LangID = int.Parse(LangId)
                //});
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
                        TypeName = "PageIndexItem",
                        TypeID = PageIndexItem.ItemID,
                         Lang_ID=int.Parse(LangId)
                    });
                }
                return "新增成功";
            }
            else
            {
                return "新增失敗";
            }

        }
        #endregion

        #region GetSelectList
        public IList<SelectListItem> GetSelectList(string id)
        {
            var list = _itemsqlrepository.GetByWhere("ModelID=@1 and Enabled=1 Order By Sort", new object[] { id });
            IList<System.Web.Mvc.SelectListItem> item = new List<System.Web.Mvc.SelectListItem>();
            if (list.Count() == 0)
            {
                var model = _sqlrepository.GetByWhere("ID=@1", new object[] { id });
                _itemsqlrepository.Create(new PageIndexItem()
                {
                    ModelID = model.First().ID,
                    ItemName = model.First().Name,
                     Lang_ID= model.First().Lang_ID,
                    Sort = 1,
                    Enabled = true,
                    UpdateUser = "system",
                    UpdateDatetime = DateTime.Now
                });
                list = _itemsqlrepository.GetByWhere("Enabled=@1 and ModelID=@2", new object[] { true, id });
            }
            foreach (var l in list)
            {
                item.Add(new System.Web.Mvc.SelectListItem() { Text = l.ItemName, Value = l.ItemID.ToString() });
            }
            return item;
        } 
        #endregion

        #region GetFirstModel
        public PageEditItemModel GetFirstModel(string id)
        {
            var data = _itemsqlrepository.GetByWhere("ModelID=@1 Order By Sort", new object[] { id }, "Top(1) *");
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var seodata = _seosqlrepository.GetByWhere("TypeName='PageIndexItem' and TypeID=@1 and Lang_ID=@2", new object[] { fdata.ItemID, data.First().Lang_ID });
                //var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                //     1, fdata.ModelID.Value,fdata.ItemID.Value
                //});
                var model = new PageEditItemModel()
                {
                    ItemID = fdata.ItemID,
                    ModelName = fdata.ItemName,
                    Description = seodata.Count() > 0 ? seodata.First().Description : "",
                    ImageFileName = fdata.ImageFileName,
                    ImageFileOrgName = fdata.ImageFileOrgName,
                    UploadFileDesc = fdata.UploadFileDesc,
                    UploadFileName = fdata.UploadFileName,
                    UploadFilePath = fdata.UploadFilePath,
                    ActiveID = fdata.ActiveID == null ? "" : fdata.ActiveID.Value.ToString(),
                    ActiveItemID= fdata.ActiveItemID == null ? "" : fdata.ActiveItemID.Value.ToString(),
                    WebsiteTitle = seodata.Count() > 0 ? seodata.First().Title : "",
                    Keywords = seodata.Count() == 0 ? new string[10] : new string[] {
                        seodata.First().Keywords1,seodata.First().Keywords2,seodata.First().Keywords3,seodata.First().Keywords4,seodata.First().Keywords5
                    ,seodata.First().Keywords6,seodata.First().Keywords7,seodata.First().Keywords8,seodata.First().Keywords9,seodata.First().Keywords10},
                    HtmlContent = fdata.HtmlContent.IsNullorEmpty()?"" : fdata.HtmlContent,
                    ImageFileDesc = fdata.ImageFileDesc,
                    ImageFileLocation = fdata.ImageFileLocation.IsNullorEmpty() ? "1" : fdata.ImageFileLocation,
                    LinkUrl = fdata.LinkUrl,
                    ModelID = int.Parse(id),
                    ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/PageEdit/" + fdata.ImageFileName),
                    CreateDatetime = fdata.CreateDatetime == null ? "" : fdata.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateUser = fdata.CreateName ,
                    UpdateDatetime= fdata.UpdateDatetime == null ? "" : fdata.UpdateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    UpdateUser= fdata.UpdateName,
                     LinkUrlDesc=fdata.LinkUrlDesc
                };
                //if (hasvdata.Count() > 0) {
                //    model.VerifyStatus = hasvdata.First().VerifyStatus == 0 ? "審核中" : (hasvdata.First().VerifyStatus == 1 ? "已通過" : "未通過");
                //    model.VerifyDateTime = hasvdata.First().VerifyDateTime == null ? "" : hasvdata.First().VerifyDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                //    model.VerifyUser = hasvdata.First().VerifyName;
                //}
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                return model;
            }
            else
            {
                PageEditItemModel model = new PageEditItemModel();
                model.Keywords = new string[10];
                model.ModelID = int.Parse(id);
                return model;
            }

        } 
        #endregion

        #region EditPageItem
        public string EditPageItem(PageEditItemModel model,string account)
        {
            var iswriteseo = false;
            //if (model.WebsiteTitle.IsNullorEmpty() == false || model.Description.IsNullorEmpty() == false
            //       || model.Keywords.Any(v => v.IsNullorEmpty() == false))
            //{iswriteseo = true; }
            var olddata = _itemsqlrepository.GetByWhere("ItemID=@1", new object[] { model.ItemID });
            var oldroot = System.Web.HttpContext.Current.Request.PhysicalApplicationPath + "\\UploadImage\\PageEdit\\";
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

            //刪除seo
            _seosqlrepository.DelDataUseWhere("TypeName='PageIndexItem' and TypeID=@1 and Lang_ID=@2", new object[] { model.ItemID, olddata.First().Lang_ID });
            var admin = _Usersqlrepository.GetByWhere("Account=@1", new object[] { account });
            var PageIndexItem = new PageIndexItem()
            {
                ItemID=model.ItemID,
                HtmlContent = model.HtmlContent==null?"": model.HtmlContent,
                ImageFileDesc = model.ImageFileDesc == null ? "" : model.ImageFileDesc,
                ImageFileLocation = model.ImageFileLocation == null ? "" : model.ImageFileLocation,
                UpdateDatetime = DateTime.Now,
                ModelID = model.ModelID,
                UpdateUser = account,
                ImageFileOrgName = model.ImageFileOrgName ,
                LinkUrl = model.LinkUrl == null ? "" : model.LinkUrl,
                LinkUrlDesc=model.LinkUrlDesc == null ? "" : model.LinkUrlDesc,
                UploadFileDesc = model.UploadFileDesc == null ? "" : model.UploadFileDesc,
                UploadFileName = model.UploadFileName,
                UploadFilePath = model.UploadFilePath == null ? "" : model.UploadFilePath,
                ImageFileName = model.ImageFileName,
                UpdateName = admin.Count() == 0 ? "" : admin.First().User_Name,
                IsVerift=true
            };
            var ModelStatus = 2;
            if (olddata.Count()==0 ||  olddata.First().CreateDatetime == null) {
                PageIndexItem.CreateDatetime = PageIndexItem.UpdateDatetime;
                PageIndexItem.CreateName = PageIndexItem.UpdateName;
                PageIndexItem.CreateUser = PageIndexItem.UpdateUser;
                ModelStatus = 1;
            }
            if (model.ActiveID.IsNullorEmpty() == false)
            {
                PageIndexItem.ActiveID = int.Parse(model.ActiveID);
            }
            if (model.ActiveItemID.IsNullorEmpty() == false)
            {
                PageIndexItem.ActiveItemID = int.Parse(model.ActiveItemID);
            }
            var r = _itemsqlrepository.Update(PageIndexItem);
            if (r > 0)
            {
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                //var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                //     1, PageIndexItem.ModelID.Value,PageIndexItem.ItemID.Value
                //});
                //if (hasvdata.Count() == 0)
                //{
                //    _verifydatasqlrepository.Create(new VerifyData()
                //    {
                //        ModelID = 1,
                //        ModelItemID = PageIndexItem.ItemID.Value,
                //        ModelName = olddata.First().ItemName,
                //        ModelMainID = PageIndexItem.ModelID.Value,
                //        VerifyStatus = 0,
                //        ModelStatus= ModelStatus,
                //        UpdateDateTime = DateTime.Now,
                //        UpdateUser = admin.Count() == 0 ? "" : admin.First().User_Name,
                //        UpdateAccount = account,
                //        LangID = olddata.First().Lang_ID.Value
                //    });
                //}
                //else {
                //    _verifydatasqlrepository.Update("VerifyStatus=0,ModelStatus=@1,ModelName =@2,VerifyDateTime=Null,VerifyUser='',VerifyName='',UpdateDateTime=@3,UpdateUser=@4,UpdateAccount=@5",
                //        "ModelID=@6 and ModelMainID=@7 and ModelItemID=@8", new object[] {
                //         ModelStatus,olddata.First().ItemName,DateTime.Now,(admin.Count() == 0 ? "" : admin.First().User_Name), account,1,PageIndexItem.ModelID.Value,PageIndexItem.ItemID.Value
                //    });
                //}
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
                        TypeName = "PageIndexItem",
                        TypeID = PageIndexItem.ItemID,
                        Lang_ID= olddata.First().Lang_ID
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
        public PageEditItemModel GetModelByID(string modelid,string itemid)
        {
            var data = _itemsqlrepository.GetByWhere("ModelID=@1 and ItemID=@2", new object[] { modelid, itemid });
            if (data.Count() > 0)
            {
                var fdata = data.First();
                var seodata = _seosqlrepository.GetByWhere("TypeName='PageIndexItem' and TypeID=@1 and Lang_ID=@2", new object[] { itemid , data .First().Lang_ID});
                var model = new PageEditItemModel()
                {
                    ItemID = fdata.ItemID,
                    ModelName= fdata.ItemName,
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
                    HtmlContent = fdata.HtmlContent.IsNullorEmpty() ? "" : fdata.HtmlContent,
                    ImageFileDesc = fdata.ImageFileDesc,
                    ImageFileLocation = fdata.ImageFileLocation.IsNullorEmpty()?"1" : fdata.ImageFileLocation,
                    LinkUrl = fdata.LinkUrl,
                    LinkUrlDesc=fdata.LinkUrlDesc,
                    ModelID = int.Parse(modelid),
                    ImageUrl = VirtualPathUtility.ToAbsolute("~/UploadImage/PageEdit/" + fdata.ImageFileName),
                    VerifyStatus = "",
                    CreateDatetime = fdata.CreateDatetime == null ? "" : fdata.CreateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    CreateUser = fdata.CreateName,
                    UpdateDatetime = fdata.UpdateDatetime == null ? "" : fdata.UpdateDatetime.Value.ToString("yyyy/MM/dd HH:mm:ss"),
                    VerifyDateTime ="",
                    UpdateUser = fdata.UpdateName,
                    VerifyUser ="",
                };
                //var hasvdata = _verifydatasqlrepository.GetByWhere("ModelID=@1 and ModelMainID=@2 and ModelItemID=@3", new object[] {
                //     1, fdata.ModelID.Value,fdata.ItemID.Value
                //});
                //if (hasvdata.Count() > 0)
                //{
                //    model.VerifyStatus = hasvdata.First().VerifyStatus == 0 ? "審核中" : (hasvdata.First().VerifyStatus == 1 ? "已通過" : "未通過");
                //    model.VerifyDateTime = hasvdata.First().VerifyDateTime == null ? "" : hasvdata.First().VerifyDateTime.Value.ToString("yyyy/MM/dd HH:mm:ss");
                //    model.VerifyUser = hasvdata.First().VerifyName;
                //}
                return model;
            }
            else
            {
                PageEditItemModel model = new PageEditItemModel();
                model.Keywords = new string[10];
                model.ModelID = int.Parse(modelid);
                return model;
            }

        }
        #endregion

        #region UpdateStatus
        public string UpdateStatus(string id, bool status, string account, string username)
        {
            try
            {
                //var entity = new PageIndexItem();
                //entity.Enabled = status ? true : false;
                //entity.UpdateDatetime = DateTime.Now;
                //entity.ItemID = int.Parse(id);
                //entity.UpdateUser = account;
                //var r = _itemsqlrepository.Update(entity);
                var r = _itemsqlrepository.Update("Enabled=@1,UpdateDatetime=@2,UpdateUser=@3", "ItemID=@4", new object[] { status, DateTime.Now, account, id });
                if (r >= 0)
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
                NLogManagement.SystemLogInfo("修改圖文編輯顯示狀態失敗:" + ex.Message);
                return "更新失敗";
            }
        }
        #endregion

        #region AddItemUnit
        public string AddItemUnit(string modelid,string name,  string account)
        {
            var model = _sqlrepository.GetByWhere("ID=@1", new object[] { modelid });
            var mexcoount = _itemsqlrepository.GetDataCaculate("Max(Sort)", "ModelID=@1", new object[] { modelid });
            var r = _itemsqlrepository.Create(new PageIndexItem()
            {
                ModelID = model.First().ID,
                ItemName = name,
                Lang_ID = model.First().Lang_ID,
                Sort = mexcoount+1,
                Enabled = true,
                UpdateUser = "system",
                UpdateDatetime = DateTime.Now
            });
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

        #region UpdateItemUnit
        public string UpdateItemUnit(string name, string id, string account)
        {
            var r = _itemsqlrepository.Update("ItemName=@1", "ItemID=@2", new object[] { name, id });
            if (r > 0) { return "修改成功"; } else { return "修改失敗"; }
        }
        #endregion

        #region UpdateItemSeq
        public string UpdateItemSeq(int modelid,int id, int seq, string type, string account, string username)
        {
            try
            {
                if (seq <= 0) { seq = 1; }
                var oldmodel = _itemsqlrepository.GetByWhere("ModelID=@1 And ItemID=@2", new object[] { modelid, id });
                if (oldmodel.Count() == 0) { return "更新失敗"; }
                var diff = "";

                diff = diff.TrimStart(',');
                var r = 0;
                if (oldmodel.First().Sort != seq)
                {
                    IList<PageIndexItem> mtseqdata = null;
                    mtseqdata = _itemsqlrepository.GetByWhere("Sort>@1 and ModelID=@2", new object[] { seq, modelid }).OrderBy(v => v.Sort).ToArray();

                    var updatetime = DateTime.Now;
                    if (mtseqdata.Count() == 0)
                    {
                        var totalcnt = 0;
                        totalcnt = _itemsqlrepository.GetCountUseWhere("ModelID=@1",new object[] { modelid });
                        seq = totalcnt;
                    }

                    var qseq = (seq > oldmodel.First().Sort) ? seq : oldmodel.First().Sort;
                    IList<PageIndexItem> ltseqdata = null;
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
                        _itemsqlrepository.Update("Sort=@1","ModelID=@2 and ItemID=@3", new object[] { sidx, modelid, tempmodel.ItemID});
                    }
                }
                //先取出大於目前seq的資料

                oldmodel.First().Sort = seq;
                r = _itemsqlrepository.Update("Sort=@1", "ModelID=@2 and ItemID=@3", new object[] { seq, modelid, oldmodel.First().ItemID });
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
                NLogManagement.SystemLogInfo("更新圖文編輯項目排序失敗:" + " error:" + ex.Message);
                return "更新圖文編輯項目排序失敗:" + " error:" + ex.Message;
            }
        }
        #endregion

        #region DeleteItem
        public string DeleteItem(string[] idlist, string delaccount, string modelid, string account, string username)
        {
            try
            {
                var r = 0;
                for (var idx = 0; idx < idlist.Length; idx++)
                {
                    var entity = new PageIndexItem();
                    entity.ItemID = int.Parse(idlist[idx]);
                    r = _itemsqlrepository.Delete(entity);
                    if (r <= 0)
                    {
                        NLogManagement.SystemLogInfo("刪除圖文編輯項目單元失敗:ID=" + idlist[idx]);
                    }
                    _seosqlrepository.DelDataUseWhere("TypeName='PageIndexItem' and TypeID=@1", new object[] { idlist[idx] });
                }
                var rstr = "";
                if (r >= 0)
                {
                    NLogManagement.SystemLogInfo("刪除圖文編輯項目單元失敗:" + delaccount);
                    rstr = "刪除成功";
                }
                else
                {
                    rstr = "刪除失敗";
                }

                var alldata = _itemsqlrepository.GetByWhere("ModelID=@1", new object[] { modelid }).OrderBy(v => v.Sort).ToArray();
                for (var idx = 1; idx <= alldata.Count(); idx++)
                {
                    var tempmodel = alldata[idx - 1];
                    tempmodel.Sort = idx;
                    _itemsqlrepository.Update(tempmodel);
                }
                _siteconfigqlrepository.Update("LastUpdateDate=@1", "", new object[] { DateTime.Now.ToString("yyyy/MM/dd") });
                return rstr;
            }
            catch (Exception ex)
            {
                NLogManagement.SystemLogInfo("刪除圖文編輯項目單元失敗:" + ex.Message);
                return "刪除失敗";
            }
        }
        #endregion

        #region GetUnitModel
        public PageUnitSettingModel GetUnitModel(string modelid)
        {
            var model = new PageUnitSettingModel();
            model.MainID = int.Parse(modelid);
            var data = _unitsqlrepository.GetByWhere("MainID=@1", new object[] { modelid });
            var maindata = _sqlrepository.GetByWhere("ID=@1", new object[] { modelid });
            if (maindata.Count() > 0) { model.Title = maindata.First().Name; }
            if (data.Count() > 0)
            {
                model.ID = data.First().ID;
                model.MainID = data.First().MainID;
                model.IsForward = data.First().IsForward;
                model.IsPrint = data.First().IsPrint;
                model.IsShare = data.First().IsShare;
                model.MemberAuth = data.First().MemberAuth;
                model.EMailAuth = data.First().EMailAuth;
                model.VIPAuth = data.First().VIPAuth;
                model.EnterpriceStudentAuth = data.First().EnterpriceStudentAuth;
                model.GeneralStudentAuth = data.First().GeneralStudentAuth;
                model.Column1= data.First().Column1;
                model.Column2 = data.First().Column2;
                model.Column3 = data.First().Column3;
                model.Column4 = data.First().Column4;
                model.Column5 = data.First().Column5;
            }
            model.ColumnNameMapping = new Dictionary<string, string>();
            model.ColumnNameMapping.Add("頁面", model.Column1.IsNullorEmpty() ? "頁面" : model.Column1);
            model.ColumnNameMapping.Add("相關連結", model.Column2.IsNullorEmpty() ? "相關連結" : model.Column2);
            model.ColumnNameMapping.Add("檔案下載", model.Column3.IsNullorEmpty() ? "檔案下載" : model.Column3);
            model.ColumnNameMapping.Add("相關圖片", model.Column4.IsNullorEmpty() ? "相關圖片" : model.Column4);
            return model;
        }
        #endregion

        #region SetUnitModel
        public string SetUnitModel(PageUnitSettingModel model, string account)
        {
            var newmodel = new PageUnitSetting();
            newmodel.UpdateDatetime = DateTime.Now;
            newmodel.UpdateUser = account;
            var r = 0;
            if (model.ID == -1)
            {
                newmodel.MainID = model.MainID.Value;
                newmodel.IsForward = model.IsForward;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsShare = model.IsShare;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                newmodel.Column1 = model.Column1==null?"" : model.Column1;
                newmodel.Column2 = model.Column2 == null ? "" : model.Column2;
                newmodel.Column3 = model.Column3 == null ? "" : model.Column3;
                newmodel.Column4 = model.Column4 == null ? "" : model.Column4;
                newmodel.Column5 = model.Column5 == null ? "" : model.Column5;
                r = _unitsqlrepository.Create(newmodel);
            }
            else
            {
                newmodel.ID = model.ID.Value;
                newmodel.MainID = model.MainID.Value;
                newmodel.IsForward = model.IsForward;
                newmodel.IsPrint = model.IsPrint;
                newmodel.IsShare = model.IsShare;
                newmodel.MemberAuth = model.MemberAuth;
                newmodel.VIPAuth = model.VIPAuth;
                newmodel.EMailAuth = model.EMailAuth;
                newmodel.EnterpriceStudentAuth = model.EnterpriceStudentAuth;
                newmodel.GeneralStudentAuth = model.GeneralStudentAuth;
                newmodel.Column1 = model.Column1 == null ? "" : model.Column1;
                newmodel.Column2 = model.Column2 == null ? "" : model.Column2;
                newmodel.Column3 = model.Column3 == null ? "" : model.Column3;
                newmodel.Column4 = model.Column4 == null ? "" : model.Column4;
                newmodel.Column5 = model.Column5 == null ? "" : model.Column5;
                r = _unitsqlrepository.Update(newmodel);
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

        #region GetFirstModel
        public IList<PageIndexItem> GetModelItem(string id)
        {

            var data = _itemsqlrepository.GetByWhere("ModelID=@1 and Enabled=1 Order By Sort", new object[] { id });
            return data.ToList();
        }
        #endregion

        #region GetFirstModel
        public PageIndexItem GetlPageItem(string itemid)
        {

            var data = _itemsqlrepository.GetByWhere("ItemID=@1  and Enabled=1  Order By Sort", new object[] { itemid });
            if (data.Count() > 0)
            {
                return data.First();
            }
            else {
                return null;
            }
         
        }
        #endregion
    }
}
